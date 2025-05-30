#include "stock_data.h"
#include <sstream>
#include <iomanip>
#include <ctime>
#include <chrono>
#include <thread>
#include <iostream>
#include <cassert>

namespace stock_data {

size_t StockData::WriteCallback(void* contents, size_t size, size_t nmemb, std::string* userp) {
    userp->append((char*)contents, size * nmemb);
    return size * nmemb;
}

StockData::StockData(const std::string& api_key)
    : api_key_(api_key)
    , curl_(curl_easy_init(), curl_easy_cleanup) {
    if (!curl_) {
        throw std::runtime_error("Failed to initialize CURL");
    }
}

StockData::~StockData() = default;



const StockData::StockInfo* StockData::getStockInfo(const std::string& ticker) {
    auto it = stock_data_.find(ticker);
    if (it != stock_data_.end()) {
        return &it->second;
    }
    // Not in cache, enforce rate limit and fetch
    if (fetchTickerData(ticker)) {
        return &stock_data_[ticker];
    }
    return nullptr;
}

bool StockData::fetchTickerData(const std::string& ticker) {
    if (!curl_) {
        return false;
    }

    std::string url = "https://api.polygon.io/v2/aggs/ticker/" + ticker + "/prev?adjusted=true&apiKey=" + api_key_;
    std::string response;

    curl_easy_setopt(curl_.get(), CURLOPT_URL, url.c_str());
    curl_easy_setopt(curl_.get(), CURLOPT_WRITEFUNCTION, WriteCallback);
    curl_easy_setopt(curl_.get(), CURLOPT_WRITEDATA, &response);

    CURLcode res = curl_easy_perform(curl_.get());
    if (res != CURLE_OK) {
        return false;
    }

    long http_code = 0;
    curl_easy_getinfo(curl_.get(), CURLINFO_RESPONSE_CODE, &http_code);
    if (http_code == 429) {
        std::cerr << "Rate limit hit for " << ticker << ", waiting 12 seconds..." << std::endl;
        std::this_thread::sleep_for(std::chrono::seconds(12));
        // Try again
        return fetchTickerData(ticker);
    }

    // Record this API call
    api_call_times_.push_back(std::chrono::steady_clock::now());

    std::cerr << "Got data for " << ticker << std::endl;

    return parseJsonResponse(response, ticker);
}

bool StockData::parseJsonResponse(const std::string& response, const std::string& ticker) {
    rapidjson::Document doc;
    doc.Parse(response.c_str());

    if (doc.HasParseError() || !doc.HasMember("results") || !doc["results"].IsArray()) {
        return false;
    }

    const auto& results = doc["results"].GetArray();
    if (results.Empty()) {
        return false;
    }

    const auto& latest = results[0];
    if (!latest.HasMember("c") || !latest.HasMember("h") || 
        !latest.HasMember("l") || !latest.HasMember("v") || 
        !latest.HasMember("t")) {
        return false;
    }

    // Convert timestamp to readable format
    int64_t timestamp = 0;
    if (latest["t"].IsInt64()) {
        timestamp = latest["t"].GetInt64();
    } else if (latest["t"].IsUint64()) {
        timestamp = static_cast<int64_t>(latest["t"].GetUint64());
    } else if (latest["t"].IsDouble()) {
        timestamp = static_cast<int64_t>(latest["t"].GetDouble());
    }
    std::time_t time = timestamp / 1000; // Convert from milliseconds to seconds
    std::tm* tm = std::localtime(&time);
    std::stringstream ss;
    ss << std::put_time(tm, "%Y-%m-%d %H:%M:%S");

    int64_t volume = 0;
    if (latest["v"].IsInt64()) {
        volume = latest["v"].GetInt64();
    } else if (latest["v"].IsUint64()) {
        volume = static_cast<int64_t>(latest["v"].GetUint64());
    } else if (latest["v"].IsDouble()) {
        volume = static_cast<int64_t>(latest["v"].GetDouble());
    }

    stock_data_[ticker] = {
        latest["c"].GetDouble(),  // Close price
        latest["h"].GetDouble(),  // High price
        latest["l"].GetDouble(),  // Low price
        latest["o"].GetDouble(),  // Open price
        volume,                   // Volume
        ss.str()                  // Formatted timestamp
    };

    return true;
}

} // namespace stock_data 