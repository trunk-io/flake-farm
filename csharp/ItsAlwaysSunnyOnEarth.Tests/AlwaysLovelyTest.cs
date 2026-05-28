using System.Threading.Tasks;
using Xunit;
using ItsAlwaysSunnyOnEarth;

namespace ItsAlwaysSunnyOnEarth.Tests
{
    public class AlwaysLovelyTest
    {
        // These tests are "flaky" because they depend on real-time weather.
        // They assert calm, clear conditions, and a comfortable temperature (55–75°F). If any fail, the test "fails".

        [Theory]
        [MemberData(nameof(Program.GetCityTestData), MemberType = typeof(Program))]
        public async Task TestIsLovely(string cityName)
        {
            bool isCalm = await Program.IsCalmInCity(cityName, 20.0);
            bool isNotCloudy = !await Program.IsCloudyInCity(cityName);
            bool isNotRaining = !await Program.IsRainingInCity(cityName);
            bool isNotSnowing = !await Program.IsSnowingInCity(cityName);
            bool isNotFoggy = !await Program.IsFoggyInCity(cityName);
            bool isNotTooCold = !await Program.IsTooColdInCity(cityName);
            bool isNotTooHot = !await Program.IsTooHotInCity(cityName);
            Assert.True(isCalm, $"Test fails if windspeed in {cityName} is >= 20 km/h.");
            Assert.True(isNotCloudy, $"Test fails if it's cloudy in {cityName}.");
            Assert.True(isNotRaining, $"Test fails if it's raining in {cityName}.");
            Assert.True(isNotSnowing, $"Test fails if it's snowing in {cityName}.");
            Assert.True(isNotFoggy, $"Test fails if it's foggy in {cityName}.");
            Assert.True(isNotTooCold, $"Test fails if temperature in {cityName} is <= 55°F.");
            Assert.True(isNotTooHot, $"Test fails if temperature in {cityName} is >= 75°F.");
        }
    }
}
