"""Tests for the stock_data module."""

import os
import random
import re
import time
import unittest

from bazel.python.src.stock_data import RateLimitError, StockData, StockDataManager

RATE_LIMIT_BACKOFF_SECONDS = 12


def get_api_key() -> str:
    """Get API key from environment variable."""
    api_key = os.environ.get("POLYGON_API_KEY")
    if not api_key:
        raise ValueError("POLYGON_API_KEY environment variable is required")
    return api_key


def _company_test_slug(company_name: str) -> str:
    slug = company_name.lower().replace("&", "and")
    slug = re.sub(r"[^a-z0-9]+", "_", slug)
    return slug.strip("_")


def _make_is_up_test(ticker: str, company_name: str):
    def test_method(self):
        info = self.manager.get_stock_info(ticker)
        self.assertIsNotNone(info, f"Failed to fetch data for {ticker}")
        self.assertGreater(
            info.close_price,
            info.open_price,
            f"{company_name} stock is down",
        )

    test_method.__name__ = f"test_{_company_test_slug(company_name)}_is_up"
    test_method.__doc__ = f"Test that {company_name} stock is up."
    return test_method


class StockDataTest(unittest.TestCase):
    """Test cases for StockData class."""

    @classmethod
    def setUpClass(cls):
        """Load all tracked tickers in one batched Polygon API call."""
        stock_data = StockData(get_api_key())
        cls.manager = StockDataManager(stock_data)
        try:
            cls.manager.load_tracked_tickers()
        except RateLimitError:
            time.sleep(RATE_LIMIT_BACKOFF_SECONDS)
            cls._rate_limited = True
            return
        cls._rate_limited = False

    def setUp(self):
        if getattr(self.__class__, "_rate_limited", False):
            self.fail(
                "Polygon rate limit hit while fetching grouped daily bars"
            )

    def test_tracked_tickers_configured(self):
        """Test that tracked ticker metadata is internally consistent."""
        self.assertEqual(
            len(StockData.TRACKED_TICKERS),
            len(StockData.TRACKED_TICKER_NAMES),
        )
        for ticker in StockData.TRACKED_TICKERS:
            self.assertIn(ticker, StockData.TRACKED_TICKER_NAMES)


for _ticker in StockData.TRACKED_TICKERS:
    _company_name = StockData.TRACKED_TICKER_NAMES[_ticker]
    _test_name = f"test_{_company_test_slug(_company_name)}_is_up"
    setattr(
        StockDataTest,
        _test_name,
        _make_is_up_test(_ticker, _company_name),
    )


def load_tests(loader, tests, pattern):
    """Randomize test order so rate-limited runs don't always hit the same tickers."""
    test_list = list(tests)
    random.shuffle(test_list)
    return unittest.TestSuite(test_list)


if __name__ == "__main__":
    unittest.main()
