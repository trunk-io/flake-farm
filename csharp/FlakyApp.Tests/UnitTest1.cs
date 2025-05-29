using Xunit;

namespace FlakyApp.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestSometimesWorks()
        {
            Assert.True(Program.SometimesWorks());
        }
    }
}
