using Xunit;
using System.Threading.Tasks;
using ItsAlwaysSunnyOnEarth;

namespace ItsAlwaysSunnyOnEarth.Tests
{
    public class AlwaysSunnyTest
    {
        // These tests are "flaky" because they depend on real-time weather.
        // They assert that it's NOT cloudy. If it IS cloudy, the test "fails".

        [Theory]
        [MemberData(nameof(Program.GetCityTestData), MemberType = typeof(Program))]
        public async Task TestWeather_NotCloudy(string cityName)
        {
            bool isCloudy = await Program.IsCloudyInCity(cityName);
            Assert.False(isCloudy, $"Test assertion fails if {cityName} is cloudy at the moment of test execution.");
        }
    }
}
