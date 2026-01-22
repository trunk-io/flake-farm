"""Stock data fetcher using Polygon.io API."""

import json
import time
import urllib.error
import urllib.request
from dataclasses import dataclass
from datetime import datetime
from typing import Dict, Optional


@dataclass
class StockInfo:
    """Stock information data structure."""

    close_price: float
    high_price: float
    low_price: float
    open_price: float
    volume: int
    timestamp: str


class StockData:
    """Fetches and caches stock data from Polygon.io API."""

    # Top 10 US companies by market cap
    TOP_COMPANIES = [
        "AAPL",  # Apple
        "MSFT",  # Microsoft
        "GOOGL",  # Alphabet
        "AMZN",  # Amazon
        "NVDA",  # NVIDIA
        "META",  # Meta
        "BRK.B",  # Berkshire Hathaway
        "TSLA",  # Tesla
        "UNH",  # UnitedHealth
        "JNJ",  # Johnson & Johnson
    ]

    def __init__(self, api_key: str):
        """Initialize StockData with API key."""
        self.api_key = api_key
        self.stock_data: Dict[str, StockInfo] = {}
        self.api_call_times = []

    def get_stock_info(self, ticker: str) -> Optional[StockInfo]:
        """Get stock info for a ticker, fetching if not cached."""
        if ticker in self.stock_data:
            return self.stock_data[ticker]
        # Not in cache, enforce rate limit and fetch
        if self._fetch_ticker_data(ticker):
            return self.stock_data[ticker]
        return None

    def get_all_stock_data(self) -> Dict[str, StockInfo]:
        """Get all cached stock data."""
        return self.stock_data

    def _fetch_ticker_data(self, ticker: str) -> bool:
        """Fetch ticker data from Polygon.io API."""
        url = (
            f"https://api.polygon.io/v2/aggs/ticker/{ticker}/prev"
            f"?adjusted=true&apiKey={self.api_key}"
        )

        try:
            with urllib.request.urlopen(url) as response:
                http_code = response.getcode()
                if http_code == 429:
                    print(f"Rate limit hit for {ticker}, waiting 12 seconds...")
                    time.sleep(12)
                    # Try again
                    return self._fetch_ticker_data(ticker)

                # Record this API call
                self.api_call_times.append(time.time())

                print(f"Got data for {ticker}")

                response_data = response.read().decode("utf-8")
                return self._parse_json_response(response_data, ticker)
        except urllib.error.HTTPError as e:
            if e.code == 429:
                print(f"Rate limit hit for {ticker}, waiting 12 seconds...")
                time.sleep(12)
                return self._fetch_ticker_data(ticker)
            return False
        except Exception:
            return False

    def _parse_json_response(self, response: str, ticker: str) -> bool:
        """Parse JSON response and populate stock_data."""
        try:
            doc = json.loads(response)

            if "results" not in doc or not isinstance(doc["results"], list):
                return False

            results = doc["results"]
            if not results:
                return False

            latest = results[0]
            required_fields = ["c", "h", "l", "v", "t", "o"]
            if not all(field in latest for field in required_fields):
                return False

            # Convert timestamp to readable format
            timestamp_ms = int(latest["t"])
            timestamp_s = timestamp_ms / 1000  # Convert from milliseconds to seconds
            timestamp_str = datetime.fromtimestamp(timestamp_s).strftime(
                "%Y-%m-%d %H:%M:%S"
            )

            volume = int(latest["v"])

            self.stock_data[ticker] = StockInfo(
                close_price=float(latest["c"]),
                high_price=float(latest["h"]),
                low_price=float(latest["l"]),
                open_price=float(latest["o"]),
                volume=volume,
                timestamp=timestamp_str,
            )

            return True
        except (json.JSONDecodeError, KeyError, ValueError, TypeError):
            return False


class StockDataManager:
    """Helper class for managing stock data fetching."""

    def __init__(self, stock_data: StockData):
        """Initialize with a StockData instance."""
        self.stock_data = stock_data

    def get_all_data(self) -> Dict[str, StockInfo]:
        """Get all stock data."""
        return self.stock_data.get_all_stock_data()

    def get_stock_info(self, ticker: str) -> Optional[StockInfo]:
        """Get stock info for a ticker."""
        return self.stock_data.get_stock_info(ticker)

    def print_stock_data(self) -> None:
        """Print formatted stock data."""
        all_data = self.get_all_data()
        print("\nTop 10 Companies Stock Data:")
        print("---------------------------")
        for ticker, info in all_data.items():
            change = info.close_price - info.open_price
            percent_change = (change / info.open_price) * 100.0
            print(f"{ticker}:")
            print(f"  Open:  ${info.open_price:.2f}")
            print(f"  Close: ${info.close_price:.2f}")
            print(f"  Change: ${change:.2f} ({percent_change:.1f}%)")
            print()
