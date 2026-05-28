"""Stock data fetcher using Polygon.io API."""

import json
import os
import time
import urllib.error
import urllib.parse
import urllib.request
from dataclasses import dataclass
from datetime import date, datetime, timedelta
from typing import Dict, Iterable, List, Optional, Set


class RateLimitError(Exception):
    """Raised when Polygon.io returns HTTP 429."""

    def __init__(self, ticker: Optional[str] = None):
        self.ticker = ticker
        if ticker:
            super().__init__(f"Polygon rate limit hit for {ticker}")
        else:
            super().__init__("Polygon rate limit hit")


@dataclass
class StockInfo:
    """Stock information data structure."""

    close_price: float
    high_price: float
    low_price: float
    open_price: float
    volume: int
    timestamp: str


def _load_tracked_ticker_names() -> Dict[str, str]:
    tickers_path = os.path.join(os.path.dirname(__file__), "tracked_tickers.json")
    with open(tickers_path, encoding="utf-8") as handle:
        names = json.load(handle)
    if not isinstance(names, dict):
        raise ValueError("tracked_tickers.json must contain a ticker-to-name mapping")
    return names


class StockData:
    """Fetches and caches stock data from Polygon.io API."""

    POLYGON_BASE = "https://api.polygon.io"

    TRACKED_TICKER_NAMES = _load_tracked_ticker_names()
    TRACKED_TICKERS: List[str] = list(TRACKED_TICKER_NAMES.keys())

    # Polygon may use alternate symbols in grouped daily responses.
    TICKER_SYMBOL_ALIASES = {
        "BF.B": ["BF.B", "BF-B"],
        "BRK.B": ["BRK.B", "BRK-B"],
    }

    # Backward-compatible alias
    TOP_COMPANIES = TRACKED_TICKERS

    def __init__(self, api_key: str):
        """Initialize StockData with API key."""
        self.api_key = api_key
        self.stock_data: Dict[str, StockInfo] = {}
        self.api_call_times = []
        self._tracked_data_loaded = False

    def load_tracked_tickers(self) -> None:
        """Fetch all tracked tickers in as few API calls as possible."""
        if self._tracked_data_loaded:
            return
        self._fetch_tracked_data_batch()
        self._tracked_data_loaded = True

    def get_stock_info(self, ticker: str) -> Optional[StockInfo]:
        """Get stock info for a ticker, fetching the batch if needed."""
        self.load_tracked_tickers()
        return self.stock_data.get(ticker)

    def get_all_stock_data(self) -> Dict[str, StockInfo]:
        """Get all cached stock data."""
        return self.stock_data

    def _fetch_tracked_data_batch(self) -> None:
        """Load tracked tickers from Polygon grouped daily bars (one call per day tried)."""
        tracked_set = set(self.TRACKED_TICKERS)

        for days_back in range(1, 8):
            trade_date = date.today() - timedelta(days=days_back)
            try:
                doc = self._request_grouped_daily(trade_date.isoformat())
            except RateLimitError:
                raise

            results = doc.get("results")
            if not isinstance(results, list) or not results:
                continue

            loaded = self._parse_grouped_daily_results(results, tracked_set)
            if loaded:
                print(
                    f"Loaded {len(loaded)} tracked tickers from grouped daily "
                    f"({trade_date.isoformat()})"
                )
                return

        print("Warning: grouped daily fetch did not return any tracked tickers")

    def _request_grouped_daily(self, trade_date: str) -> dict:
        """Call Polygon grouped daily endpoint for a single market date."""
        query = urllib.parse.urlencode(
            {"adjusted": "true", "apiKey": self.api_key}
        )
        url = (
            f"{self.POLYGON_BASE}/v2/aggs/grouped/locale/us/market/stocks/"
            f"{trade_date}?{query}"
        )

        try:
            with urllib.request.urlopen(url) as response:
                if response.getcode() == 429:
                    raise RateLimitError()
                self.api_call_times.append(time.time())
                response_data = response.read().decode("utf-8")
                return json.loads(response_data)
        except RateLimitError:
            raise
        except urllib.error.HTTPError as e:
            if e.code == 429:
                raise RateLimitError() from e
            raise
        except (json.JSONDecodeError, urllib.error.URLError):
            return {}

    def _parse_grouped_daily_results(
        self, results: Iterable[dict], tracked_set: Set[str]
    ) -> Set[str]:
        """Parse grouped daily rows into stock_data; return loaded tracked tickers."""
        rows_by_symbol = {
            row.get("T"): row for row in results if isinstance(row, dict) and row.get("T")
        }
        loaded: Set[str] = set()

        for ticker in self.TRACKED_TICKERS:
            row = self._lookup_grouped_row(rows_by_symbol, ticker)
            if row is None:
                continue
            info = self._stock_info_from_aggregate(row)
            if info is None:
                continue
            self.stock_data[ticker] = info
            loaded.add(ticker)

        return loaded

    def _lookup_grouped_row(
        self, rows_by_symbol: Dict[str, dict], ticker: str
    ) -> Optional[dict]:
        symbols = self.TICKER_SYMBOL_ALIASES.get(ticker, [ticker])
        for symbol in symbols:
            row = rows_by_symbol.get(symbol)
            if row is not None:
                return row
        return None

    def _stock_info_from_aggregate(self, latest: dict) -> Optional[StockInfo]:
        required_fields = ["c", "h", "l", "v", "t", "o"]
        if not all(field in latest for field in required_fields):
            return None

        timestamp_ms = int(latest["t"])
        timestamp_s = timestamp_ms / 1000
        timestamp_str = datetime.fromtimestamp(timestamp_s).strftime(
            "%Y-%m-%d %H:%M:%S"
        )

        return StockInfo(
            close_price=float(latest["c"]),
            high_price=float(latest["h"]),
            low_price=float(latest["l"]),
            open_price=float(latest["o"]),
            volume=int(latest["v"]),
            timestamp=timestamp_str,
        )


class StockDataManager:
    """Helper class for managing stock data fetching."""

    def __init__(self, stock_data: StockData):
        """Initialize with a StockData instance."""
        self.stock_data = stock_data

    def load_tracked_tickers(self) -> None:
        """Fetch all tracked tickers in one batched API call when possible."""
        self.stock_data.load_tracked_tickers()

    def get_all_data(self) -> Dict[str, StockInfo]:
        """Get all stock data."""
        return self.stock_data.get_all_stock_data()

    def get_stock_info(self, ticker: str) -> Optional[StockInfo]:
        """Get stock info for a ticker."""
        return self.stock_data.get_stock_info(ticker)

    def print_stock_data(self) -> None:
        """Print formatted stock data."""
        all_data = self.get_all_data()
        print("\nTracked Companies Stock Data:")
        print("---------------------------")
        for ticker, info in all_data.items():
            change = info.close_price - info.open_price
            percent_change = (change / info.open_price) * 100.0
            print(f"{ticker}:")
            print(f"  Open:  ${info.open_price:.2f}")
            print(f"  Close: ${info.close_price:.2f}")
            print(f"  Change: ${change:.2f} ({percent_change:.1f}%)")
            print()
