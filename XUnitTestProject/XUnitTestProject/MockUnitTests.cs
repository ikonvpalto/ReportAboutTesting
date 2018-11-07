using Xunit;
using Moq;

namespace XUnitTestProject
{
    public class MockUnitTests
    {
        [Fact]
        public void TestOrderIsFilledIfEnoughInWarehouse()
        {
            Mock<IWarehouse> warehouseMock = new Mock<IWarehouse>();
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
            Mock<IWarehouse> warehouseMock = new Mock<IWarehouse>();
            warehouseMock
                .Setup(warehouse => warehouse.IsHave("1", 6))
                .Returns(false);
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
