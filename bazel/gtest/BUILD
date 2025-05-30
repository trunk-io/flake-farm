load("@rules_cc//cc:defs.bzl", "cc_test")

cc_test(
    name = "lottery_test",
    size = "small",
    srcs = [
        "lottery_test.cc",
        "lottery.h",
    ],
    deps = [
        "@googletest//:gtest",
        "@googletest//:gtest_main",
        "@curl//:curl",
        "@nlohmann_json//:json",
    ],
)
