using Xunit;
using System.Threading.Tasks;
using ItsAlwaysSunnyOnEarth; // Assuming Program class is in ItsAlwaysSunnyOnEarth namespace

namespace ItsAlwaysSunnyOnEarth.Tests
{
    public class AlwaysSunnyTest // Renamed class to match file name
    {
        // These tests are "flaky" because they depend on real-time weather.
        // They assert that it's NOT cloudy. If it IS cloudy, the test "fails".

        [Fact]
        public async Task TestWeather_NewYork()
        {
            bool isCloudy = await Program.IsCloudyInCity("New York");
            Assert.False(isCloudy, "Test assertion fails if New York is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_London()
        {
            bool isCloudy = await Program.IsCloudyInCity("London");
            Assert.False(isCloudy, "Test assertion fails if London is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_Paris()
        {
            bool isCloudy = await Program.IsCloudyInCity("Paris");
            Assert.False(isCloudy, "Test assertion fails if Paris is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_Tokyo()
        {
            bool isCloudy = await Program.IsCloudyInCity("Tokyo");
            Assert.False(isCloudy, "Test assertion fails if Tokyo is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_Beijing()
        {
            bool isCloudy = await Program.IsCloudyInCity("Beijing");
            Assert.False(isCloudy, "Test assertion fails if Beijing is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_Moscow()
        {
            bool isCloudy = await Program.IsCloudyInCity("Moscow");
            Assert.False(isCloudy, "Test assertion fails if Moscow is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_Cairo()
        {
            bool isCloudy = await Program.IsCloudyInCity("Cairo");
            Assert.False(isCloudy, "Test assertion fails if Cairo is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_Sydney()
        {
            bool isCloudy = await Program.IsCloudyInCity("Sydney");
            Assert.False(isCloudy, "Test assertion fails if Sydney is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_RioDeJaneiro()
        {
            bool isCloudy = await Program.IsCloudyInCity("Rio de Janeiro");
            Assert.False(isCloudy, "Test assertion fails if Rio de Janeiro is cloudy at the moment of test execution.");
        }

        [Fact]
        public async Task TestWeather_Berlin()
        {
            bool isCloudy = await Program.IsCloudyInCity("Berlin");
            Assert.False(isCloudy, "Test assertion fails if Berlin is cloudy at the moment of test execution.");
        }
    }
}
