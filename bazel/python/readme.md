# Bazel Python Example

This directory contains a simple Python example demonstrating Bazel's Python testing capabilities.

## Structure

- `src/calculator.py` - A simple Calculator class with basic arithmetic operations
- `tests/calculator_test.py` - Unit tests using Python's `unittest` framework

## Running Tests

To run the tests:

```bash
bazel test //bazel/python/tests:calculator_test
```

To run with Build Event Protocol output (for CI integration):

```bash
bazel test //bazel/python/tests:calculator_test --build_event_json_file=build_events.json
```

## Test Coverage

The tests cover:

- Addition
- Subtraction
- Multiplication
- Division
- Division by zero error handling
