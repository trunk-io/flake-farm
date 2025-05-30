#pragma once

#include <string>
#include <vector>
#include <map>
#include <memory>
#include <curl/curl.h>
#include <rapidjson/document.h>
#include <iostream>
#include <iomanip>
#include <chrono>

namespace stock_data {

class StockData {
public:
    struct StockInfo {
        double close_price;
        double high_price;
        double low_price;
        double open_price;
        int64_t volume;
        std::string timestamp;
    };

    explicit StockData(const std::string& api_key);
    virtual ~StockData();

    // Fetch and cache on demand
    virtual const StockInfo* getStockInfo(const std::string& ticker);
    const std::map<std::string, StockInfo>& getAllStockData() const { return stock_data_; }

protected:
    static size_t WriteCallback(void* contents, size_t size, size_t nmemb, std::string* userp);
    bool parseJsonResponse(const std::string& response, const std::string& ticker);
    bool fetchTickerData(const std::string& ticker);

    std::string api_key_;
    std::unique_ptr<CURL, decltype(&curl_easy_cleanup)> curl_;
    std::map<std::string, StockInfo> stock_data_;
    std::vector<std::chrono::steady_clock::time_point> api_call_times_;
    
    // Top 10 US companies by market cap
    const std::vector<std::string> top_companies_ = {
        "AAPL",  // Apple
        "MSFT",  // Microsoft
        "GOOGL", // Alphabet
        "AMZN",  // Amazon
        "NVDA",  // NVIDIA
        "META",  // Meta
        "BRK.B", // Berkshire Hathaway
        "TSLA",  // Tesla
        "UNH",   // UnitedHealth
        "JNJ"    // Johnson & Johnson
    };
};

// Helper class for managing stock data fetching
class StockDataManager {
public:
    explicit StockDataManager(std::unique_ptr<StockData> stock_data)
        : stock_data_(std::move(stock_data)) {}

    const std::map<std::string, StockData::StockInfo>& getAllData() const {
        return stock_data_->getAllStockData();
    }

    const StockData::StockInfo* getStockInfo(const std::string& ticker) const {
        return stock_data_->getStockInfo(ticker);
    }

    void printStockData() const {
        const auto& all_data = getAllData();
        std::cout << "\nTop 10 Companies Stock Data:" << std::endl;
        std::cout << "---------------------------" << std::endl;
        for (const auto& [ticker, info] : all_data) {
            double change = info.close_price - info.open_price;
            double percent_change = (change / info.open_price) * 100.0;
            std::cout << ticker << ":\n"
                      << "  Open:  $" << std::fixed << std::setprecision(2) << info.open_price << "\n"
                      << "  Close: $" << info.close_price << "\n"
                      << "  Change: $" << change << " (" << std::setprecision(1) << percent_change << "%)\n"
                      << std::endl;
        }
    }

private:
    std::unique_ptr<StockData> stock_data_;
};

} // namespace stock_data 