using System;
using System.Threading;
using Xunit;

namespace ParkingLot.Tests
{
    public class ParkingLotTestsBilka
    {
        readonly IParkingLot lot;
        readonly TestClock clock;

        public ParkingLotTestsBilka()
        {
            clock = new TestClock();
            lot = new ParkingLot(clock, 20m, TimeSpan.FromMinutes(30), TimeSpan.Zero);
            Gates.Reset();
        }

        [Fact]
        public void Owes10WhenPaid10()
        {
            lot.Checkin("AB 12 123");
            clock.Forward(TimeSpan.FromMilliseconds(1));
            lot.BeginCheckout("AB 12 123");

            lot.Pay("AB 12 123", 10m);

            Assert.Equal(10m, lot.GetRemainingFee("AB 12 123"));
        }

        [Fact]
        public void ItCosts20ToPark24Minutes()
        {
            lot.Checkin("AB 12 123");

            clock.Forward(TimeSpan.FromMinutes(24));
            lot.BeginCheckout("AB 12 123");

            Assert.Equal(20m, lot.GetRemainingFee("AB 12 123"));
        }

        [Fact]
        public void ItCosts20ToPark30Minutes()
        {
            lot.Checkin("AB 12 123");

            clock.Forward(TimeSpan.FromMinutes(30));
            lot.BeginCheckout("AB 12 123");

            Assert.Equal(20m, lot.GetRemainingFee("AB 12 123"));
        }

        [Fact]
        public void ItCosts40ToPark30MinutesAnd1Ms()
        {
            lot.Checkin("AB 12 123");

            clock.Forward(TimeSpan.FromMinutes(30) + TimeSpan.FromMilliseconds(1));
            lot.BeginCheckout("AB 12 123");

            Assert.Equal(40m, lot.GetRemainingFee("AB 12 123"));
        }
    }
}
