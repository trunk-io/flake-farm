#include "stock_data.h"
#include "../config.h"
#include <gtest/gtest.h>
#include <gmock/gmock.h>
#include <iostream>
#include <iomanip>

namespace stock_data {
namespace test {

class StockDataTest : public ::testing::Test {
protected:
    static void SetUpTestSuite() {
        std::cout << "\nFetching live stock data...\n" << std::endl;
        manager_ = std::make_unique<StockDataManager>(std::make_unique<StockData>(config::getApiKey()));
        
        // Fetch data for all companies
        for (const auto& ticker : {"AAPL", "MSFT", "GOOGL", "AMZN", "NVDA", "META", "BRK.B", "TSLA", "UNH", "JNJ"}) {
            ASSERT_NE(manager_->getStockInfo(ticker), nullptr) << "Failed to fetch data for " << ticker;
        }
        
        manager_->printStockData();
    }

    static void TearDownTestSuite() {
        manager_.reset();
    }

    static std::unique_ptr<StockDataManager> manager_;
};

std::unique_ptr<StockDataManager> StockDataTest::manager_;

TEST_F(StockDataTest, AllCompaniesDataFetched) {
    const auto& all_data = manager_->getAllData();
    EXPECT_EQ(all_data.size(), 10);
    
    for (const auto& [ticker, info] : all_data) {
        EXPECT_GT(info.close_price, 0.0);
        EXPECT_GE(info.high_price, info.low_price);
        EXPECT_GT(info.volume, 0);
        EXPECT_FALSE(info.timestamp.empty());
    }
}

// Individual company tests
TEST_F(StockDataTest, AppleIsUp) {
    const auto* info = manager_->getStockInfo("AAPL");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Apple stock is down";
}

TEST_F(StockDataTest, MicrosoftIsUp) {
    const auto* info = manager_->getStockInfo("MSFT");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Microsoft stock is down";
}

TEST_F(StockDataTest, GoogleIsUp) {
    const auto* info = manager_->getStockInfo("GOOGL");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Google stock is down";
}

TEST_F(StockDataTest, AmazonIsUp) {
    const auto* info = manager_->getStockInfo("AMZN");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Amazon stock is down";
}

TEST_F(StockDataTest, NvidiaIsUp) {
    const auto* info = manager_->getStockInfo("NVDA");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Nvidia stock is down";
}

TEST_F(StockDataTest, MetaIsUp) {
    const auto* info = manager_->getStockInfo("META");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Meta stock is down";
}

TEST_F(StockDataTest, BerkshireIsUp) {
    const auto* info = manager_->getStockInfo("BRK.B");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Berkshire stock is down";
}

TEST_F(StockDataTest, TeslaIsUp) {
    const auto* info = manager_->getStockInfo("TSLA");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Tesla stock is down";
}

TEST_F(StockDataTest, UnitedHealthIsUp) {
    const auto* info = manager_->getStockInfo("UNH");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "UnitedHealth stock is down";
}

TEST_F(StockDataTest, JohnsonAndJohnsonIsUp) {
    const auto* info = manager_->getStockInfo("JNJ");
    ASSERT_NE(info, nullptr);
    EXPECT_GT(info->close_price, info->open_price) << "Johnson & Johnson stock is down";
}

} // namespace test
} // namespace stock_data 