"""Tests for the stock_data module (500 S&P 500 tickers)."""

import os
import random
import re
import sys
import time
import traceback
import unittest
import xml.etree.ElementTree as ET

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
            self.fail("Polygon rate limit hit while fetching grouped daily bars")

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


class _RecordingResult(unittest.TestResult):
    """Collects a per-test outcome so each ticker can be emitted as its own JUnit testcase."""

    def __init__(self):
        super().__init__()
        self.records = []
        self._start_times = {}

    def startTest(self, test):
        super().startTest(test)
        self._start_times[test] = time.time()

    def _elapsed(self, test):
        return time.time() - self._start_times.get(test, time.time())

    def addSuccess(self, test):
        super().addSuccess(test)
        self.records.append((test, "success", "", "", self._elapsed(test)))

    def addFailure(self, test, err):
        super().addFailure(test, err)
        self.records.append(
            (test, "failure", err[0].__name__, _format_err(err), self._elapsed(test))
        )

    def addError(self, test, err):
        super().addError(test, err)
        self.records.append(
            (test, "error", err[0].__name__, _format_err(err), self._elapsed(test))
        )

    def addSkip(self, test, reason):
        super().addSkip(test, reason)
        self.records.append((test, "skipped", "", reason, self._elapsed(test)))


def _format_err(err) -> str:
    return "".join(traceback.format_exception(*err))


def _summary_line(message: str) -> str:
    """Return the final non-empty traceback line, i.e. the 'ExcType: message' summary."""
    if not message:
        return ""
    for line in reversed(message.strip().splitlines()):
        stripped = line.strip()
        if stripped:
            return stripped
    return ""


def _write_junit_xml(
    output_path: str, result: _RecordingResult, total_time: float
) -> None:
    """Write one <testcase> per ticker so Bazel/Trunk track each ticker as a distinct test."""
    outcomes = [outcome for _, outcome, _, _, _ in result.records]
    testsuites = ET.Element("testsuites")
    testsuite = ET.SubElement(
        testsuites,
        "testsuite",
        {
            "name": "StockDataTest",
            "tests": str(len(result.records)),
            "failures": str(outcomes.count("failure")),
            "errors": str(outcomes.count("error")),
            "skipped": str(outcomes.count("skipped")),
            "time": f"{total_time:.3f}",
        },
    )

    for test, outcome, exc_type, message, elapsed in result.records:
        test_id = test.id()
        classname, _, name = test_id.rpartition(".")
        testcase = ET.SubElement(
            testsuite,
            "testcase",
            {"name": name, "classname": classname, "time": f"{elapsed:.3f}"},
        )
        if outcome in ("failure", "error"):
            node = ET.SubElement(
                testcase, outcome, {"message": _summary_line(message), "type": exc_type}
            )
            node.text = message
        elif outcome == "skipped":
            ET.SubElement(testcase, "skipped", {"message": message or ""})

    tree = ET.ElementTree(testsuites)
    ET.indent(tree)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def main() -> None:
    # Bazel sets XML_OUTPUT_FILE and, when the test does not populate it, falls back
    # to a single synthesized testcase for the whole target. Emit our own JUnit XML so
    # every ticker surfaces as an individual test.
    xml_output_file = os.environ.get("XML_OUTPUT_FILE")
    if not xml_output_file:
        unittest.main()
        return

    suite = unittest.TestLoader().loadTestsFromTestCase(StockDataTest)
    ordered = list(suite)
    random.shuffle(ordered)

    result = _RecordingResult()
    started = time.time()
    unittest.TestSuite(ordered).run(result)
    _write_junit_xml(xml_output_file, result, time.time() - started)

    sys.exit(0 if result.wasSuccessful() else 1)


if __name__ == "__main__":
    main()
