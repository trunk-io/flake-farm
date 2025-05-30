load("@rules_cc//cc:cc_library.bzl", "cc_library")

cc_library(
    name = "stock_data",
    srcs = ["src/stock_data.cpp"],
    hdrs = [
        "include/stock_data.h",
        "config.h",
    ],
    includes = ["include"],
    deps = [
        "@curl//:curl",
        "@rapidjson//:rapidjson",
    ],
    visibility = ["//visibility:public"],
    copts = [
        "-Wall",
        "-Wextra",
        "-Werror",
    ],
) 