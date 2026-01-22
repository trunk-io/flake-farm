using System.Threading.Tasks;
using Xunit;
using ItsAlwaysSunnyOnEarth; // Assuming Program.cs is in this namespace

namespace ItsAlwaysSunnyOnEarth.Tests
{
    public class AlwaysLovelyTest
    {
        // Test method for New York
        [Fact]
        public async Task TestIsLovely_NewYork()
        {
            bool isCalm = await Program.IsCalmInCity("New York", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("New York");
            Assert.True(isCalm, "Test fails if windspeed in New York is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in New York.");
        }

        // Test method for London
        [Fact]
        public async Task TestIsLovely_London()
        {
            bool isCalm = await Program.IsCalmInCity("London", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("London");
            Assert.True(isCalm, "Test fails if windspeed in London is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in London.");
        }

        // Test method for Paris
        [Fact]
        public async Task TestIsLovely_Paris()
        {
            bool isCalm = await Program.IsCalmInCity("Paris", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("Paris");
            Assert.True(isCalm, "Test fails if windspeed in Paris is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in Paris.");
        }

        // Test method for Tokyo
        [Fact]
        public async Task TestIsLovely_Tokyo()
        {
            bool isCalm = await Program.IsCalmInCity("Tokyo", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("Tokyo");
            Assert.True(isCalm, "Test fails if windspeed in Tokyo is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in Tokyo.");
        }

        // Test method for Beijing
        [Fact]
        public async Task TestIsLovely_Beijing()
        {
            bool isCalm = await Program.IsCalmInCity("Beijing", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("Beijing");
            Assert.True(isCalm, "Test fails if windspeed in Beijing is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in Beijing.");
        }

        // Test method for Moscow
        [Fact]
        public async Task TestIsLovely_Moscow()
        {
            bool isCalm = await Program.IsCalmInCity("Moscow", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("Moscow");
            Assert.True(isCalm, "Test fails if windspeed in Moscow is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in Moscow.");
        }

        // Test method for Cairo
        [Fact]
        public async Task TestIsLovely_Cairo()
        {
            bool isCalm = await Program.IsCalmInCity("Cairo", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("Cairo");
            Assert.True(isCalm, "Test fails if windspeed in Cairo is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in Cairo.");
        }

        // Test method for Sydney
        [Fact]
        public async Task TestIsLovely_Sydney()
        {
            bool isCalm = await Program.IsCalmInCity("Sydney", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("Sydney");
            Assert.True(isCalm, "Test fails if windspeed in Sydney is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in Sydney.");
        }

        // Test method for Rio de Janeiro
        [Fact]
        public async Task TestIsLovely_RioDeJaneiro() // Corrected city name
        {
            bool isCalm = await Program.IsCalmInCity("Rio de Janeiro", 10.0); // Corrected city name
            bool isNotCloudy = !await Program.IsCloudyInCity("Rio de Janeiro");
            Assert.True(isCalm, "Test fails if windspeed in Rio de Janeiro is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in Rio de Janeiro.");
        }

        // Test method for Berlin
        [Fact]
        public async Task TestIsLovely_Berlin()
        {
            bool isCalm = await Program.IsCalmInCity("Berlin", 10.0);
            bool isNotCloudy = !await Program.IsCloudyInCity("Berlin");
            Assert.True(isCalm, "Test fails if windspeed in Berlin is >= 10 km/h.");
            Assert.True(isNotCloudy, "Test fails if it's cloudy in Berlin.");
        }
    }
}
