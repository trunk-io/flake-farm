"""Tests for the stock_data module."""

import os
import re
import unittest

from bazel.python.src.stock_data import StockData, StockDataManager


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
        self.assertIsNotNone(info)
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
        """Set up test fixtures - fetch live stock data."""
        print("\nFetching live stock data...\n")
        stock_data = StockData(get_api_key())
        cls.manager = StockDataManager(stock_data)

        for ticker in StockData.TRACKED_TICKERS:
            info = cls.manager.get_stock_info(ticker)
            assert info is not None, f"Failed to fetch data for {ticker}"

        cls.manager.print_stock_data()

    def test_all_companies_data_fetched(self):
        """Test that all companies' data was fetched."""
        all_data = self.manager.get_all_data()
        self.assertEqual(len(all_data), len(StockData.TRACKED_TICKERS))

        for _ticker, info in all_data.items():
            self.assertGreater(info.close_price, 0.0)
            self.assertGreaterEqual(info.high_price, info.low_price)
            self.assertGreater(info.volume, 0)
            self.assertFalse(info.timestamp == "")


for _ticker in StockData.TRACKED_TICKERS:
    _company_name = StockData.TRACKED_TICKER_NAMES[_ticker]
    _test_name = f"test_{_company_test_slug(_company_name)}_is_up"
    setattr(
        StockDataTest,
        _test_name,
        _make_is_up_test(_ticker, _company_name),
    )


if __name__ == "__main__":
    unittest.main()
