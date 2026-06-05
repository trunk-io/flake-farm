<!-- markdownlint-disable first-line-heading -->
<img width="750" height="750" alt="image" src="https://github.com/user-attachments/assets/5ee22177-5e5a-48ec-9ce7-0734d8ce60ab" />


[![docs](https://img.shields.io/badge/-docs-darkgreen?logo=readthedocs&logoColor=ffffff)][docs]
[![slack](https://img.shields.io/badge/-slack-611f69?logo=slack)][slack]

### Welcome

This repository is designed to generate flaky test results. Many of the tests are written poorly
with intent to flake.

### Demonstrated testing frameworks

| Language   | Frameworks                                | Workflow                                                                                               |
| ---------- | ----------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| Python     | pytest, robotframework, behave            | [Python Tests](.github/workflows/python-tests.yaml), [Retry Tests](.github/workflows/retry-tests.yaml) |
| JavaScript | mocha, jasmine, jest, playwright          | [JavaScript Tests](.github/workflows/javascript-tests.yaml)                                            |
| Java       | JUnit (Gradle/Maven), Playwright          | [Java Tests](.github/workflows/java-tests.yaml)                                                        |
| Go         | go test (go-junit-report, gotestsum)      | [Go Tests](.github/workflows/go.yaml)                                                                  |
| PHP        | phpunit                                   | [PHP Tests](.github/workflows/php.yaml)                                                                |
| Ruby       | rspec (with trunk rspec plugin), minitest | [Ruby Tests](.github/workflows/ruby-tests.yaml)                                                        |
| Rust       | nextest                                   | [Rust Tests](.github/workflows/rust-tests.yaml)                                                        |
| C#         | xUnit                                     | [C# Tests](.github/workflows/csharp-tests.yaml)                                                        |
| Bazel      | gtest                                     | [Bazel Tests](.github/workflows/bazel.yaml)                                                            |

### Mission

Our goal is to make engineering faster, more efficient and dare we say - more fun. This repository
will hopefully allow our community to share ideas on the best tools and best practices/workflows to
make everyone's job of building code a little bit easier, a little bit faster, and maybe in the
process - a little bit more fun. Read more about [Trunk Flaky](https://trunk.io/flaky-tests).

[slack]: https://slack.trunk.io
[docs]: https://docs.trunk.io
[vscode]: https://marketplace.visualstudio.com/items?itemName=Trunk.io
