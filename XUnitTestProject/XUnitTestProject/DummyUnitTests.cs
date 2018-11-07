using Xunit;

namespace XUnitTestProject
{
    public class DummyUnitTests
    {
        internal class DummyLogger : ILogger
        {
            public void Log(string message) { }
        }

        [Fact]
        public void TestWithDummy()
        {
            ILogger dummy = new DummyLogger();
            Calculator calculator = new Calculator(dummy);

            int result = calculator.Sum(1, 5, -1);

            Assert.Equal(5, result);
        }
    }
}
