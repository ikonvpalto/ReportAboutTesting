using System;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTestProject
{
    public class Fixture
    {
//        public 
    }

    public class FixtureTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public FixtureTests(ITestOutputHelper output)
        {
            this._output = output;
            output.WriteLine("Constructor");
        }

        [Fact]
        public void TestSimple()
        {
            _output.WriteLine("TestSimple");
        }

        [Fact]
        public void TestException()
        {
            _output.WriteLine("TestException");
        }

        public void Dispose()
        {
            _output.WriteLine("Destructor");
        }
    }
}
