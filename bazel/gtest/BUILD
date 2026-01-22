load("@rules_cc//cc:cc_library.bzl", "cc_library")

cc_library(
    name = "stock_data",
    srcs = ["src/stock_data.cpp"],
    hdrs = [
        "config.h",
        "include/stock_data.h",
    ],
    copts = [
        "-Wall",
        "-Wextra",
        "-Werror",
    ],
    includes = ["include"],
    visibility = ["//visibility:public"],
    deps = [
        "@curl",
        "@rapidjson",
    ],
)
