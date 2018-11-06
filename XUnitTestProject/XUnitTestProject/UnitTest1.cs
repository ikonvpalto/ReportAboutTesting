using Xunit;
using Moq;

namespace XUnitTestProject
{
    public class StubUnitTests
    {
        [Fact]
        public void TestOrderIsFilledIfEnoughInWarehouse()
        {
            Warehouse warehouse = new Warehouse { { "1", 5 }, { "2", 6 } };
            Order order = new Order
            {
                Amount = 5,
                Product = "1"
            };


            order.Fill(warehouse);

            Assert.True(order.IsFilled);
            Assert.Equal(0, warehouse["1"]);
        }

        [Fact]
        public void TestOrderDoesNotRemoveIfNotEnough()
        {
            Warehouse warehouse = new Warehouse { { "1", 5 }, { "2", 6 } };
            Order order = new Order
            {
                Amount = 6,
                Product = "1"
            };


            order.Fill(warehouse);

            Assert.False(order.IsFilled);
            Assert.Equal(5, warehouse["1"]);
        }
    }

    public class MockUnitTests
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

        [Fact]
        public void TestOrderIsFilledIfEnoughInWarehouse()
        {
            Mock<Warehouse> warehouseMock = new Mock<Warehouse>();
            warehouseMock
                .Setup(warehouse => warehouse.IsHave("1", 5))
                .Returns(true);
            Order order = new Order
            {
                Amount = 5,
                Product = "1"
            };

            order.Fill(warehouseMock.Object);

            Assert.True(order.IsFilled);
            warehouseMock.Verify(warehouse => warehouse.IsHave("1", 5), Times.Once);
            warehouseMock.Verify(warehouse => warehouse.IsHave(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            warehouseMock.Verify(warehouse => warehouse.Take("1", 5), Times.Once);
            warehouseMock.Verify(warehouse => warehouse.Take(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void TestOrderDoesNotRemoveIfNotEnough()
        {
            Mock<Warehouse> warehouseMock = new Mock<Warehouse>();
            warehouseMock
                .Setup(warehouse => warehouse.IsHave("1", 6))
                .Returns(true);
            Order order = new Order
            {
                Amount = 6,
                Product = "1"
            };


            order.Fill(warehouseMock.Object);

            Assert.False(order.IsFilled);
            warehouseMock.Verify(warehouse => warehouse.IsHave("1", 6), Times.Once);
            warehouseMock.Verify(warehouse => warehouse.IsHave(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            warehouseMock.Verify(warehouse => warehouse.Take(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }
    }
}
