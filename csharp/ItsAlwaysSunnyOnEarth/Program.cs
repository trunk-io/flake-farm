using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; // Required for GetFromJsonAsync
using System.Text.Json.Serialization; // Required for JsonPropertyName
using System.Threading.Tasks;

namespace ItsAlwaysSunnyOnEarth
{
    public class Program
    {
        private static readonly Dictionary<string, (double Latitude, double Longitude)> CityCoordinates = new()
        {
            { "Accra", (5.56, -0.19) },
            { "Addis Ababa", (9.03, 38.74) },
            { "Almaty", (43.24, 76.95) },
            { "Amsterdam", (52.37, 4.90) },
            { "Athens", (37.98, 23.73) },
            { "Atlanta", (33.75, -84.39) },
            { "Auckland", (-36.85, 174.76) },
            { "Bangalore", (12.97, 77.59) },
            { "Bangkok", (13.76, 100.50) },
            { "Barcelona", (41.39, 2.17) },
            { "Beijing", (39.91, 116.40) },
            { "Berlin", (52.52, 13.41) },
            { "Bogota", (4.71, -74.07) },
            { "Boston", (42.36, -71.06) },
            { "Brisbane", (-27.47, 153.03) },
            { "Buenos Aires", (-34.60, -58.38) },
            { "Budapest", (47.50, 19.04) },
            { "Cairo", (30.05, 31.23) },
            { "Cape Town", (-33.92, 18.42) },
            { "Casablanca", (33.57, -7.59) },
            { "Chennai", (13.08, 80.27) },
            { "Chicago", (41.88, -87.63) },
            { "Copenhagen", (55.68, 12.57) },
            { "Dakar", (14.72, -17.47) },
            { "Dallas", (32.78, -96.80) },
            { "Delhi", (28.61, 77.21) },
            { "Denver", (39.74, -104.99) },
            { "Dhaka", (23.81, 90.41) },
            { "Doha", (25.29, 51.53) },
            { "Dubai", (25.20, 55.27) },
            { "Dublin", (53.35, -6.26) },
            { "Guadalajara", (20.67, -103.35) },
            { "Guangzhou", (23.13, 113.26) },
            { "Hanoi", (21.03, 105.85) },
            { "Havana", (23.11, -82.37) },
            { "Helsinki", (60.17, 24.94) },
            { "Ho Chi Minh City", (10.82, 106.63) },
            { "Honolulu", (21.31, -157.86) },
            { "Hong Kong", (22.32, 114.17) },
            { "Houston", (29.76, -95.37) },
            { "Hyderabad", (17.39, 78.49) },
            { "Istanbul", (41.01, 28.98) },
            { "Jakarta", (-6.21, 106.85) },
            { "Johannesburg", (-26.20, 28.04) },
            { "Karachi", (24.86, 67.01) },
            { "Kinshasa", (-4.32, 15.31) },
            { "Kolkata", (22.57, 88.36) },
            { "Kuala Lumpur", (3.14, 101.69) },
            { "Lagos", (6.52, 3.38) },
            { "Lima", (-12.05, -77.04) },
            { "Lisbon", (38.72, -9.14) },
            { "London", (51.51, -0.13) },
            { "Los Angeles", (34.05, -118.24) },
            { "Luanda", (-8.84, 13.23) },
            { "Madrid", (40.42, -3.70) },
            { "Manila", (14.60, 120.98) },
            { "Medellin", (6.25, -75.56) },
            { "Melbourne", (-37.81, 144.96) },
            { "Mexico City", (19.43, -99.13) },
            { "Miami", (25.76, -80.19) },
            { "Milan", (45.46, 9.19) },
            { "Montevideo", (-34.90, -56.16) },
            { "Montreal", (45.50, -73.57) },
            { "Moscow", (55.75, 37.62) },
            { "Mumbai", (19.08, 72.88) },
            { "Munich", (48.14, 11.58) },
            { "Nairobi", (1.29, 36.82) },
            { "New York", (40.71, -74.01) },
            { "Osaka", (34.69, 135.50) },
            { "Paris", (48.85, 2.35) },
            { "Perth", (-31.95, 115.86) },
            { "Philadelphia", (39.95, -75.17) },
            { "Phoenix", (33.45, -112.07) },
            { "Prague", (50.08, 14.44) },
            { "Rio de Janeiro", (-22.91, -43.17) },
            { "Riyadh", (24.69, 46.72) },
            { "Rome", (41.90, 12.50) },
            { "San Diego", (32.72, -117.16) },
            { "San Francisco", (37.77, -122.42) },
            { "Santiago", (-33.45, -70.67) },
            { "Sao Paulo", (-23.55, -46.63) },
            { "Seattle", (47.61, -122.33) },
            { "Seoul", (37.57, 126.98) },
            { "Shanghai", (31.23, 121.47) },
            { "Singapore", (1.35, 103.82) },
            { "Stockholm", (59.33, 18.07) },
            { "Sydney", (-33.87, 151.21) },
            { "Taipei", (25.03, 121.57) },
            { "Tashkent", (41.30, 69.28) },
            { "Tehran", (35.69, 51.39) },
            { "Tel Aviv", (32.09, 34.78) },
            { "Tokyo", (35.69, 139.69) },
            { "Toronto", (43.65, -79.38) },
            { "Tunis", (36.81, 10.18) },
            { "Vancouver", (49.28, -123.12) },
            { "Vienna", (48.21, 16.37) },
            { "Washington", (38.91, -77.04) },
            { "Warsaw", (52.23, 21.01) },
            { "Wellington", (-41.29, 174.78) },
            { "Zurich", (47.37, 8.54) }
        };

        public static IEnumerable<object[]> GetCityTestData()
        {
            foreach (var city in CityCoordinates.Keys)
            {
                yield return new object[] { city };
            }
        }

        // User-Agent for HTTP requests
        private const string UserAgent = "ItsAlwaysSunnyOnEarthApp/1.1 (github.com/your-repo/ItsAlwaysSunnyOnEarth)";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Weather Checker: ItsAlwaysSunnyOnEarth Edition v1.1");
            List<string> citiesToCheck = new List<string>();

            if (args.Length > 0)
            {
                citiesToCheck.AddRange(args);
            }
            else
            {
                // Example usage if no arguments are provided
                citiesToCheck.AddRange(new[] { "London", "Tokyo", "NonExistentCity", "Berlin" });
            }

            foreach (var city in citiesToCheck)
            {
                Console.WriteLine($"--- Checking: {city} ---");
                bool isCloudy = await IsCloudyInCity(city);
                Console.WriteLine(isCloudy ? $"It's likely CLOUDY in {city}." : $"It's likely NOT CLOUDY in {city}.");

                bool isDaytime = await IsDaytimeInCity(city);
                Console.WriteLine(isDaytime ? $"It's currently DAYTIME in {city}." : $"It's currently NIGHTTIME in {city}.");

                bool isCalm = await IsCalmInCity(city); // Default threshold 5.0
                Console.WriteLine(isCalm ? $"It's CALM in {city} (wind < 5 km/h)." : $"It's WINDY in {city} (wind >= 5 km/h).");
            }
        }

        private static async Task<CurrentWeather?> GetCurrentWeatherAsync(string cityName)
        {
            if (!CityCoordinates.TryGetValue(cityName, out var coords))
            {
                Console.WriteLine($"City '{cityName}' not found in our list. Coordinates unavailable.");
                return null;
            }

            double latitude = coords.Latitude;
            double longitude = coords.Longitude;
            string apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitude:0.00}&longitude={longitude:0.00}&current_weather=true";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                try
                {
                    // Console.WriteLine($"Requesting weather data from: {apiUrl}"); // Optional: for debugging
                    WeatherResponse? weatherData = await client.GetFromJsonAsync<WeatherResponse>(apiUrl);
                    if (weatherData != null && weatherData.CurrentWeatherInfo != null)
                    {
                        return weatherData.CurrentWeatherInfo;
                    }
                    else
                    {
                        Console.WriteLine($"No current weather data returned or parsed correctly for {cityName}.");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"HTTP request error for {cityName}: {e.Message}");
                    if (e.StatusCode.HasValue) Console.WriteLine($"Status code: {e.StatusCode.Value}");
                }
                catch (Exception e) // Catch other errors like JSON deserialization issues
                {
                    Console.WriteLine($"An unexpected error occurred while fetching weather for {cityName}: {e.Message}");
                }
            }
            return null; // Return null if any error occurs or data is not available
        }

        public static async Task<bool> IsCloudyInCity(string cityName)
        {
            CurrentWeather? currentWeather = await GetCurrentWeatherAsync(cityName);
            if (currentWeather != null)
            {
                // WMO Weather interpretation codes: 3, 45, 48 indicate cloudy/foggy conditions.
                // Reference: https://open-meteo.com/en/docs (Weather variable: weather_code)
                Console.WriteLine($"Cloud check for {cityName}: Weather code {currentWeather.WeatherCode}");
                return currentWeather.WeatherCode == 3 || currentWeather.WeatherCode == 45 || currentWeather.WeatherCode == 48;
            }
            return false; // Default to false if weather data couldn't be retrieved
        }

        public static async Task<bool> IsDaytimeInCity(string cityName)
        {
            CurrentWeather? currentWeather = await GetCurrentWeatherAsync(cityName);
            if (currentWeather != null)
            {
                Console.WriteLine($"Daytime check for {cityName}: Is_Day code {currentWeather.Is_Day}");
                return currentWeather.Is_Day == 1;
            }
            return false; // Default to false (e.g., interpret as night) if data unavailable
        }

        public static async Task<bool> IsCalmInCity(string cityName, double windSpeedThreshold = 5.0)
        {
            CurrentWeather? currentWeather = await GetCurrentWeatherAsync(cityName);
            if (currentWeather != null)
            {
                Console.WriteLine($"Wind speed check for {cityName}: Windspeed {currentWeather.Windspeed} km/h");
                return currentWeather.Windspeed < windSpeedThreshold;
            }
            return false; // Default to false (e.g., interpret as windy) if data unavailable
        }
    }

    public class WeatherResponse
    {
        [JsonPropertyName("current_weather")]
        public CurrentWeather? CurrentWeatherInfo { get; set; }
        
        // Other properties from the response if needed
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("windspeed")]
        public double Windspeed { get; set; } // Added

        [JsonPropertyName("winddirection")]
        public int WindDirection { get; set; }

        [JsonPropertyName("weathercode")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("is_day")]
        public int Is_Day { get; set; } // Added

        [JsonPropertyName("time")]
        public string? Time { get; set; }
    }
}
