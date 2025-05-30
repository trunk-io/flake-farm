load("@rules_cc//cc:cc_test.bzl", "cc_test")

cc_test(
    name = "stock_data_test",
    srcs = ["stock_data_test.cpp"],
    deps = [
        "//bazel/stock_data:stock_data",
        "@googletest//:gtest_main",
    ],
) 