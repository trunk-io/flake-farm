#pragma once
#include <string>

namespace stock_data {
namespace config {

// Get API key from environment variable, with a default for testing
inline std::string getApiKey() {
    const char* env_key = std::getenv("POLYGON_API_KEY");
    return env_key ? env_key : "dWJwiSUGqP7sZtIJYy9Vko2uCACcKaCD";
}

} // namespace config
} // namespace stock_data 