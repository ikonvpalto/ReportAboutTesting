using Xunit;

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
}