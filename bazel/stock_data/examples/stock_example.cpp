#include "stock_data.h"
#include "../config.h"
#include <iostream>
#include <iomanip>

int main() {
    try {
        stock_data::StockData stock_data(stock_data::config::POLYGON_API_KEY);
        
        std::cout << "Fetching stock data for top 10 US companies..." << std::endl;
        if (!stock_data.fetchTopCompaniesData()) {
            std::cerr << "Failed to fetch some stock data" << std::endl;
        }

        std::cout << "\nStock Data Summary:\n" << std::endl;
        std::cout << std::setw(8) << "Ticker" << " | "
                  << std::setw(12) << "Close" << " | "
                  << std::setw(12) << "High" << " | "
                  << std::setw(12) << "Low" << " | "
                  << std::setw(15) << "Volume" << " | "
                  << "Timestamp" << std::endl;
        std::cout << std::string(80, '-') << std::endl;

        for (const auto& [ticker, info] : stock_data.getAllStockData()) {
            std::cout << std::setw(8) << ticker << " | "
                      << std::setw(12) << std::fixed << std::setprecision(2) << info.close_price << " | "
                      << std::setw(12) << info.high_price << " | "
                      << std::setw(12) << info.low_price << " | "
                      << std::setw(15) << info.volume << " | "
                      << info.timestamp << std::endl;
        }

    } catch (const std::exception& e) {
        std::cerr << "Error: " << e.what() << std::endl;
        return 1;
    }

    return 0;
} 