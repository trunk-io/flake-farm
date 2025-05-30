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
            { "New York", (40.71, -74.01) },
            { "London", (51.51, -0.13) },
            { "Paris", (48.85, 2.35) },
            { "Tokyo", (35.69, 139.69) },
            { "Beijing", (39.91, 116.40) },
            { "Moscow", (55.75, 37.62) },
            { "Cairo", (30.05, 31.23) },
            { "Sydney", (-33.87, 151.21) },
            { "Rio de Janeiro", (-22.91, -43.17) },
            { "Berlin", (52.52, 13.41) }
        };

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
