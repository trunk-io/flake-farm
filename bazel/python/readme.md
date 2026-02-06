# Bazel Python Example

This directory contains a Python example demonstrating Bazel's Python testing capabilities with flaky tests.

## Structure

- `src/stock_data.py` - Stock data fetcher using Polygon.io API
- `tests/stock_data_test.py` - Tests for stock data fetching (includes flaky tests)

## Running Tests

To run the tests:

```bash
bazel test //bazel/python/tests:stock_data_test
```

To run with Build Event Protocol output (for CI integration):

```bash
bazel test //bazel/python/tests:stock_data_test --build_event_json_file=build_events.json
```

## Test Coverage

### Stock Data Tests

- Fetches live stock data for top 10 companies
- Validates data structure and values
- Includes flaky tests that check if stocks are "up" (close > open)
