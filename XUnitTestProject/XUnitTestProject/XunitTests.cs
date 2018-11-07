using Xunit;

namespace XUnitTestProject
{
    public class UnitTests
    {
        [Fact]
        public void TestSimple()
        {
            int num = 1 + 2;

            Assert.Equal(3, num);
        }
    }
}
