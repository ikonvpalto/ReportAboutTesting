using Xunit;
using System;

namespace XUnitTestProject
{
    public class XunitTests
    {
        [Fact]
        public void TestSimple()
        {
            int num = 1 + 2;

            Assert.Equal(3, num);
        }

        [Fact]
        public void TestException()
        {
            string a = null;

            NullReferenceException throwedException =
                Assert.Throws<NullReferenceException>(() => a.Insert(1, "asd"));

            Assert.Equal(nameof(XUnitTestProject), throwedException.Source);
        }

        public int q = 1;

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(0, 0, 0)]
        [InlineData(0, int.MaxValue, int.MaxValue)]
        [InlineData(int.MinValue, int.MaxValue, -1)]
        [InlineData(int.MaxValue, int.MaxValue, -2)]
        [InlineData(int.MinValue, int.MinValue, 0)]
        public void TestTheorySimple(int a, int b, int expected)
        {
            int num = a + b;

            Assert.Equal(expected, num);
        }
    }
}