using System;
using ParkingLot.Fotex;
using Xunit;

namespace ParkingLot.Tests
{
    public class FotexPriceStrategyTests
    {
        readonly IPriceStrategy strategy = new FotexPriceStrategy();

        [Fact]
        public void PriceFor6minutes()
        {
            Assert.Equal(15, strategy.CalcalatePrice(TimeSpan.FromMinutes(6)));
        }
    }
}
