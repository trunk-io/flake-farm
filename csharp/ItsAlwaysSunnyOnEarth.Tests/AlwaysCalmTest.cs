using System.Threading.Tasks;
using Xunit;
using ItsAlwaysSunnyOnEarth; // Assuming Program.cs is in this namespace

namespace ItsAlwaysSunnyOnEarth.Tests
{
    public class AlwaysCalmTest
    {
        // Test method for New York
        [Fact]
        public async Task TestIsCalm_NewYork()
        {
            bool isCalm = await Program.IsCalmInCity("New York");
            Assert.True(isCalm, "Test fails if windspeed in New York is >= 5 km/h.");
        }

        // Test method for London
        [Fact]
        public async Task TestIsCalm_London()
        {
            bool isCalm = await Program.IsCalmInCity("London");
            Assert.True(isCalm, "Test fails if windspeed in London is >= 5 km/h.");
        }

        // Test method for Paris
        [Fact]
        public async Task TestIsCalm_Paris()
        {
            bool isCalm = await Program.IsCalmInCity("Paris");
            Assert.True(isCalm, "Test fails if windspeed in Paris is >= 5 km/h.");
        }

        // Test method for Tokyo
        [Fact]
        public async Task TestIsCalm_Tokyo()
        {
            bool isCalm = await Program.IsCalmInCity("Tokyo");
            Assert.True(isCalm, "Test fails if windspeed in Tokyo is >= 5 km/h.");
        }

        // Test method for Beijing
        [Fact]
        public async Task TestIsCalm_Beijing()
        {
            bool isCalm = await Program.IsCalmInCity("Beijing");
            Assert.True(isCalm, "Test fails if windspeed in Beijing is >= 5 km/h.");
        }

        // Test method for Moscow
        [Fact]
        public async Task TestIsCalm_Moscow()
        {
            bool isCalm = await Program.IsCalmInCity("Moscow");
            Assert.True(isCalm, "Test fails if windspeed in Moscow is >= 5 km/h.");
        }

        // Test method for Cairo
        [Fact]
        public async Task TestIsCalm_Cairo()
        {
            bool isCalm = await Program.IsCalmInCity("Cairo");
            Assert.True(isCalm, "Test fails if windspeed in Cairo is >= 5 km/h.");
        }

        // Test method for Sydney
        [Fact]
        public async Task TestIsCalm_Sydney()
        {
            bool isCalm = await Program.IsCalmInCity("Sydney");
            Assert.True(isCalm, "Test fails if windspeed in Sydney is >= 5 km/h.");
        }

        // Test method for Rio de Janeiro
        [Fact]
        public async Task TestIsCalm_RioDeJaneiro() // Corrected city name
        {
            bool isCalm = await Program.IsCalmInCity("Rio de Janeiro"); // Corrected city name
            Assert.True(isCalm, "Test fails if windspeed in Rio de Janeiro is >= 5 km/h.");
        }

        // Test method for Berlin
        [Fact]
        public async Task TestIsCalm_Berlin()
        {
            bool isCalm = await Program.IsCalmInCity("Berlin");
            Assert.True(isCalm, "Test fails if windspeed in Berlin is >= 5 km/h.");
        }
    }
}
