<!-- markdownlint-disable first-line-heading -->

[![docs](https://img.shields.io/badge/-docs-darkgreen?logo=readthedocs&logoColor=ffffff)][docs]
[![slack](https://img.shields.io/badge/-slack-611f69?logo=slack)][slack]

### Welcome

This repository is designed to generate flaky test results. Many of the tests are written poorly
with intent to flake.

### Demonstrated testing frameworks

| Language   | Frameworks                           | Workflow                                                                                               |
| ---------- | ------------------------------------ | ------------------------------------------------------------------------------------------------------ |
| python     | pytest, robotframework, behave       | [Python Tests](.github/workflows/python-tests.yaml), [Retry Tests](.github/workflows/retry-tests.yaml) |
| javascript | mocha, jasmine, jest, playwright     | [JavaScript Tests](.github/workflows/javascript-tests.yaml)                                            |
| java       | JUnit (Gradle/Maven), Playwright     | [Java Tests](.github/workflows/java-tests.yaml)                                                        |
| go         | go test (go-junit-report, gotestsum) | [Go Tests](.github/workflows/go.yaml)                                                                  |
| php        | phpunit                              | [PHP Tests](.github/workflows/php.yaml)                                                                |
| ruby       | rspec, minitest                      | [Ruby Tests](.github/workflows/ruby-tests.yaml)                                                        |
| rust       | nextest                              | [Rust Tests](.github/workflows/rust-tests.yaml)                                                        |
| bazel      | gtest                                | [Bazel Tests](.github/workflows/bazel.yaml)                                                            |

### Mission

Our goal is to make engineering faster, more efficient and dare we say - more fun. This repository
will hopefully allow our community to share ideas on the best tools and best practices/workflows to
make everyone's job of building code a little bit easier, a little bit faster, and maybe in the
process - a little bit more fun. Read more about [Trunk Flaky](https://trunk.io/flaky-tests).

[slack]: https://slack.trunk.io
[docs]: https://docs.trunk.io
[vscode]: https://marketplace.visualstudio.com/items?itemName=Trunk.io
