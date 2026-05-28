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


def _fail_on_rate_limit(test_case, company_name: str, ticker: str) -> None:
    """Fail this test immediately on 429, then sleep before the next test runs."""
    test_case.addCleanup(lambda: time.sleep(RATE_LIMIT_BACKOFF_SECONDS))
    test_case.fail(
        f"Polygon rate limit hit for {company_name} ({ticker})"
    )


def _make_is_up_test(ticker: str, company_name: str):
    def test_method(self):
        try:
            info = self.manager.get_stock_info(ticker)
        except RateLimitError:
            _fail_on_rate_limit(self, company_name, ticker)
            return

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
        """Set up shared manager; each test fetches its own ticker."""
        stock_data = StockData(get_api_key())
        cls.manager = StockDataManager(stock_data)

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
