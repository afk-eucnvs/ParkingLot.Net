using System;
using System.Threading;
using Xunit;

namespace ParkingLot.Tests
{
    public class ParkingLotTestsNetto
    {
        readonly IParkingLot lot;
        readonly TestClock clock;

        public ParkingLotTestsNetto()
        {
            clock = new TestClock();
            lot = new ParkingLot(clock, 15m, TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30));
            Gates.Reset();
        }

        [Fact]
        public void Owes5WhenPaid10()
        {
            lot.Checkin("AB 12 123");
            clock.Forward(TimeSpan.FromMinutes(31));
            lot.BeginCheckout("AB 12 123");

            lot.Pay("AB 12 123", 10m);

            Assert.Equal(5m, lot.GetRemainingFee("AB 12 123"));
        }

        [Fact]
        public void ItCosts0ToPark24Minutes()
        {
            lot.Checkin("AB 12 123");

            clock.Forward(TimeSpan.FromMinutes(24));
            lot.BeginCheckout("AB 12 123");

            Assert.Equal(0m, lot.GetRemainingFee("AB 12 123"));
        }

        [Fact]
        public void ItCosts0ToPark30Minutes()
        {
            lot.Checkin("AB 12 123");

            clock.Forward(TimeSpan.FromMinutes(30));
            lot.BeginCheckout("AB 12 123");

            Assert.Equal(0m, lot.GetRemainingFee("AB 12 123"));
        }

        [Fact]

        public void ItCosts15ToPark30Minutes1Millisecond()
        {
            lot.Checkin("AB 12 123");

            clock.Forward(TimeSpan.FromMinutes(30) + TimeSpan.FromMilliseconds(1));
            lot.BeginCheckout("AB 12 123");

            Assert.Equal(15m, lot.GetRemainingFee("AB 12 123"));
        }
    }
}
