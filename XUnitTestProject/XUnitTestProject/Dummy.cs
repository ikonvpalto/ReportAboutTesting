using System;
using System.Linq;

namespace XUnitTestProject
{
    internal class Calculator
    {
        private ILogger _logger;

        public Calculator(ILogger logger)
        {
            _logger = logger;
        }

        public int Sum(params int[] numbers)
        {
            return numbers.Sum();
        }

    }

    internal interface ILogger
    {
        void Log(string message);
    }

}
