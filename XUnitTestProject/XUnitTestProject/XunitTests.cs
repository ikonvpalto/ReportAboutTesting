using Xunit;
using System;
using System.Collections.Generic;
using System.Collections;

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

        public static List<object[]> testParams = new List<object[]>
        {
            new object[] {1, 2, 3},
            new object[] {0, 0, 0},
            new object[] {0, int.MaxValue, int.MaxValue},
            new object[] {int.MinValue, int.MaxValue, -1},
            new object[] {int.MaxValue, int.MaxValue, -2},
            new object[] {int.MinValue, int.MinValue, 0},
        };

        [Theory]
        [MemberData(nameof(testParams))]
        public void TestTheoryMember(int a, int b, int expected)
        {
            int num = a + b;

            Assert.Equal(expected, num);
        }

        public static IEnumerable<object[]> generateTestParams() 
        {
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { 0, 0, 0 };
            yield return new object[] { 0, int.MaxValue, int.MaxValue };
            yield return new object[] { int.MinValue, int.MaxValue, -1 };
            yield return new object[] { int.MaxValue, int.MaxValue, -2 };
            yield return new object[] { int.MinValue, int.MinValue, 0 };
        }

        [Theory]
        [MemberData(nameof(generateTestParams))]
        public void TestTheoryMemberGenerator(int a, int b, int expected)
        {
            int num = a + b;

            Assert.Equal(expected, num);
        }

        public class TestsParamsGenerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 1, 2, 3 };
                yield return new object[] { 0, 0, 0 };
                yield return new object[] { 0, int.MaxValue, int.MaxValue };
                yield return new object[] { int.MinValue, int.MaxValue, -1 };
                yield return new object[] { int.MaxValue, int.MaxValue, -2 };
                yield return new object[] { int.MinValue, int.MinValue, 0 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestsParamsGenerator))]
        public void TestTheoryClass(int a, int b, int expected)
        {
            int num = a + b;

            Assert.Equal(expected, num);
        }
    }
}