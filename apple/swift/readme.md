# Example using [Swift Testing](https://github.com/swiftlang/swift-testing)

Swift Testing ships with the toolchain as of Swift 6, so `import Testing` needs no package
dependency and no developer snapshot. This package builds and tests on Linux as well as macOS —
CI runs it on `ubuntu-latest` via [`swift-tests.yaml`](../../.github/workflows/swift-tests.yaml).

## Running it

With a Swift 6 toolchain on the path:

```bash
swift build
swift test
swift test --xunit-output test-results/swift_test.xml
```

Or without installing one, using the official Linux image:

```bash
docker run --rm -v "$PWD":/work -w /work swift:6.1 \
  bash -c 'mkdir -p test-results && swift test --xunit-output test-results/swift_test.xml'
```

## Two things about the XML

**The filename is not the one you pass.** SwiftPM writes one file per testing library and inserts
the library's name before the extension, so `--xunit-output test-results/swift_test.xml` produces
`test-results/swift_test-swift-testing.xml`. The workflow globs `test-results/*.xml` rather than
naming the file.

**`@Test` and `@Suite` display names do not reach the XML.** JUnit identity is the symbol: `classname`
is `MyCLITests.<struct>` and `name` is `<function>()`. Renaming `"rad test name"` costs nothing;
renaming `helloworld()` starts a new test as far as the product is concerned.

A thrown error and a failed `#expect` both land as `<failure>` — Swift Testing's xunit output never
emits `<error>`.

## The tests fail on purpose

Every suite is a flake generator with its own pattern: an unconditional failure, a pass-rate ladder,
two clock-driven windows, an intermittent thrown error, and two always-passing tests whose duration
moves. Keep a blank line between any comment and the `@Test` or `@Suite` below it — Swift Testing
treats a contiguous preceding comment as a test comment and reprints it under every failure.
