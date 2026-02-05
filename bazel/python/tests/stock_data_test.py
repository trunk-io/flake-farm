"""Tests for the stock_data module."""

import os
import unittest

from bazel.python.src.stock_data import StockData, StockDataManager


def get_api_key() -> str:
    """Get API key from environment variable."""
    api_key = os.environ.get("POLYGON_API_KEY")
    if not api_key:
        raise ValueError("POLYGON_API_KEY environment variable is required")
    return api_key


class StockDataTest(unittest.TestCase):
    """Test cases for StockData class."""

    @classmethod
    def setUpClass(cls):
        """Set up test fixtures - fetch live stock data."""
        print("\nFetching live stock data...\n")
        stock_data = StockData(get_api_key())
        cls.manager = StockDataManager(stock_data)

        # Fetch data for all companies
        tickers = [
            "AAPL",
            "MSFT",
            "GOOGL",
            "AMZN",
            "NVDA",
            "META",
            "BRK.B",
            "TSLA",
            "UNH",
            "JNJ",
        ]
        for ticker in tickers:
            info = cls.manager.get_stock_info(ticker)
            assert info is not None, f"Failed to fetch data for {ticker}"

        cls.manager.print_stock_data()

    def test_all_companies_data_fetched(self):
        """Test that all companies' data was fetched."""
        all_data = self.manager.get_all_data()
        self.assertEqual(len(all_data), 10)

        for _ticker, info in all_data.items():
            self.assertGreater(info.close_price, 0.0)
            self.assertGreaterEqual(info.high_price, info.low_price)
            self.assertGreater(info.volume, 0)
            self.assertFalse(info.timestamp == "")

    def test_apple_is_up(self):
        """Test that Apple stock is up."""
        info = self.manager.get_stock_info("AAPL")
        self.assertIsNotNone(info)
        self.assertGreater(info.close_price, info.open_price, "Apple stock is down")

    def test_microsoft_is_up(self):
        """Test that Microsoft stock is up."""
        info = self.manager.get_stock_info("MSFT")
        self.assertIsNotNone(info)
        self.assertGreater(info.close_price, info.open_price, "Microsoft stock is down")

    def test_google_is_up(self):
        """Test that Google stock is up."""
        info = self.manager.get_stock_info("GOOGL")
        self.assertIsNotNone(info)
        self.assertGreater(info.close_price, info.open_price, "Google stock is down")

    def test_amazon_is_up(self):
        """Test that Amazon stock is up."""
        info = self.manager.get_stock_info("AMZN")
        self.assertIsNotNone(info)
        self.assertGreater(info.close_price, info.open_price, "Amazon stock is down")

    def test_nvidia_is_up(self):
        """Test that Nvidia stock is up."""
        info = self.manager.get_stock_info("NVDA")
        self.assertIsNotNone(info)
        self.assertGreater(info.close_price, info.open_price, "Nvidia stock is down")

    def test_meta_is_up(self):
        """Test that Meta stock is up."""
        info = self.manager.get_stock_info("META")
        self.assertIsNotNone(info)
        self.assertGreater(info.close_price, info.open_price, "Meta stock is down")

    def test_berkshire_is_up(self):
        """Test that Berkshire stock is up."""
        info = self.manager.get_stock_info("BRK.B")
        self.assertIsNotNone(info)
        self.assertGreater(info.close_price, info.open_price, "Berkshire stock is down")

    def test_tesla_is_up(self):
        """Test that Tesla stock is up."""
        info = self.manager.get_stock_info("TSLA")
        self.assertIsNotNone(info)
        self.assertGreater(info.close_price, info.open_price, "Tesla stock is down")

    def test_unitedhealth_is_up(self):
        """Test that UnitedHealth stock is up."""
        info = self.manager.get_stock_info("UNH")
        self.assertIsNotNone(info)
        self.assertGreater(
            info.close_price, info.open_price, "UnitedHealth stock is down"
        )

    def test_johnson_and_johnson_is_up(self):
        """Test that Johnson & Johnson stock is up."""
        info = self.manager.get_stock_info("JNJ")
        self.assertIsNotNone(info)
        self.assertGreater(
            info.close_price, info.open_price, "Johnson & Johnson stock is down"
        )


if __name__ == "__main__":
    unittest.main()
