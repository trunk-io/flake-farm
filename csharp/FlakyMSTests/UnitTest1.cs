using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FlakyMSTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestFlakyFunctionDemonstratesFlakiness()
        {
            bool exceptionThrown = false;
            int attempts = 0;
            const int maxAttempts = 10; // Run the function up to 10 times

            for (int i = 0; i < maxAttempts; i++)
            {
                attempts++;
                try
                {
                    // Assuming Program class is in the default namespace or csharp project's root namespace
                    Program.FlakyFunction(); 
                }
                catch (Exception)
                {
                    exceptionThrown = true;
                    break; // Exit loop once exception is caught
                }
            }

            Assert.IsTrue(exceptionThrown, $"FlakyFunction did not throw an exception within {maxAttempts} attempts. It executed {attempts} times.");
        }
    }
}
