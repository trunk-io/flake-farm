using System.Threading.Tasks;
using Xunit;
using ItsAlwaysSunnyOnEarth; // Assuming Program.cs is in this namespace

namespace ItsAlwaysSunnyOnEarth.Tests
{
    public class AlwaysDaytimeTest
    {
        private static readonly string[] Cities = new string[]
        {
            "New York", "London", "Paris", "Tokyo", "Beijing",
            "Moscow", "Cairo", "Sydney", "Rio de Janeiro", "Berlin"
        };

        // Test method for New York
        [Fact]
        public async Task TestIsDaytime_NewYork()
        {
            bool isDaytime = await Program.IsDaytimeInCity("New York");
            Assert.True(isDaytime, "Test fails if it's not daytime in New York.");
        }

        // Test method for London
        [Fact]
        public async Task TestIsDaytime_London()
        {
            bool isDaytime = await Program.IsDaytimeInCity("London");
            Assert.True(isDaytime, "Test fails if it's not daytime in London.");
        }

        // Test method for Paris
        [Fact]
        public async Task TestIsDaytime_Paris()
        {
            bool isDaytime = await Program.IsDaytimeInCity("Paris");
            Assert.True(isDaytime, "Test fails if it's not daytime in Paris.");
        }

        // Test method for Tokyo
        [Fact]
        public async Task TestIsDaytime_Tokyo()
        {
            bool isDaytime = await Program.IsDaytimeInCity("Tokyo");
            Assert.True(isDaytime, "Test fails if it's not daytime in Tokyo.");
        }

        // Test method for Beijing
        [Fact]
        public async Task TestIsDaytime_Beijing()
        {
            bool isDaytime = await Program.IsDaytimeInCity("Beijing");
            Assert.True(isDaytime, "Test fails if it's not daytime in Beijing.");
        }

        // Test method for Moscow
        [Fact]
        public async Task TestIsDaytime_Moscow()
        {
            bool isDaytime = await Program.IsDaytimeInCity("Moscow");
            Assert.True(isDaytime, "Test fails if it's not daytime in Moscow.");
        }

        // Test method for Cairo
        [Fact]
        public async Task TestIsDaytime_Cairo()
        {
            bool isDaytime = await Program.IsDaytimeInCity("Cairo");
            Assert.True(isDaytime, "Test fails if it's not daytime in Cairo.");
        }

        // Test method for Sydney
        [Fact]
        public async Task TestIsDaytime_Sydney()
        {
            bool isDaytime = await Program.IsDaytimeInCity("Sydney");
            Assert.True(isDaytime, "Test fails if it's not daytime in Sydney.");
        }

        // Test method for Rio de Janeiro
        [Fact]
        public async Task TestIsDaytime_RioDeJaneiro() // Corrected city name
        {
            bool isDaytime = await Program.IsDaytimeInCity("Rio de Janeiro"); // Corrected city name
            Assert.True(isDaytime, "Test fails if it's not daytime in Rio de Janeiro.");
        }

        // Test method for Berlin
        [Fact]
        public async Task TestIsDaytime_Berlin()
        {
            bool isDaytime = await Program.IsDaytimeInCity("Berlin");
            Assert.True(isDaytime, "Test fails if it's not daytime in Berlin.");
        }
    }
}
