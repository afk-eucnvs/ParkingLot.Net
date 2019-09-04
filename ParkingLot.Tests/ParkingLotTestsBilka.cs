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
        public void CanCheckin()
        {
            lot.Checkin("AB 12 123");
            Assert.Equal(1, Gates.NumberOfCars);
        }

        [Fact]
        public void CanCheckTwoCarsIn()
        {
            lot.Checkin("AB 12 123");
            lot.Checkin("AB 12 124");
            Assert.Equal(2, Gates.NumberOfCars);
        }

        [Fact]
        public void CannotCheckinTwice()
        {
            lot.Checkin("AB 12 123");

            Assert.Throws<InvalidOperationException>(() => lot.Checkin("AB 12 123"));
            Assert.Equal(1, Gates.NumberOfCars);
        }

        [Fact]
        public void CanBeginCheckout()
        {
            lot.Checkin("AB 12 123");

            lot.BeginCheckout("AB 12 123");
        }

        [Fact]
        public void CannotBeginCheckoutTwice()
        {
            lot.Checkin("AB 12 123");
            lot.BeginCheckout("AB 12 123");

            Assert.Throws<InvalidOperationException>(() => lot.BeginCheckout("AB 12 123"));
        }

        [Fact]
        public void CannotBeginCheckoutIfNotParked()
        {
            Assert.Throws<InvalidOperationException>(() => lot.BeginCheckout("AB 12 123"));
        }

        [Fact]
        public void OwesNothingWhenNotCheckedIn()
        {
            Assert.Equal(0, lot.GetRemainingFee("AB 12 123"));
        }

        [Fact]
        public void Owes15WhenBeginingCheckingOut()
        {
            lot.Checkin("AB 12 123");
            clock.Forward(TimeSpan.FromMilliseconds(1));
            lot.BeginCheckout("AB 12 123");

            Assert.Equal(20m, lot.GetRemainingFee("AB 12 123"));
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
        public void CannotLeaveWithoutPaying()
        {
            lot.Checkin("AB 12 123");
            clock.Forward(TimeSpan.FromMilliseconds(1));
            lot.BeginCheckout("AB 12 123");

            Assert.Throws<InvalidOperationException>(() => lot.Leave("AB 12 123"));
        }

        [Fact]
        public void CannotLeaveWithoutPayingFullFee()
        {
            lot.Checkin("AB 12 123");
            clock.Forward(TimeSpan.FromMilliseconds(1));
            lot.BeginCheckout("AB 12 123");

            lot.Pay("AB 12 123", 10m);

            Assert.Throws<InvalidOperationException>(() => lot.Leave("AB 12 123"));
        }

        [Fact]
        public void CanLeaveWhenFeeIsPayed()
        {
            lot.Checkin("AB 12 123");
            lot.BeginCheckout("AB 12 123");

            lot.Pay("AB 12 123", 40m);

            lot.Leave("AB 12 123");

            Assert.Equal(0, Gates.NumberOfCars);
        }


        [Fact]
        public void CanLeaveWhenMoreThanFeeIsPayed()
        {
            lot.Checkin("AB 12 123");
            clock.Forward(TimeSpan.FromMilliseconds(1));
            lot.BeginCheckout("AB 12 123");

            lot.Pay("AB 12 123", 10m);
            lot.Pay("AB 12 123", 50m);

            Assert.Equal(-40m, lot.GetRemainingFee("AB 12 123"));
            lot.Leave("AB 12 123");

            Assert.Equal(0, Gates.NumberOfCars);
        }


        [Fact]
        public void CannotPayWhenNotCheckedIn()
        {
            Assert.Throws<InvalidOperationException>(() => lot.Pay("AB 12 123", 10));
        }

        [Fact]
        public void CannotPayNegativeAmount()
        {
            lot.Checkin("AB 12 123");
            lot.BeginCheckout("AB 12 123");

            Assert.Throws<InvalidOperationException>(() => lot.Pay("AB 12 123", -10));
        }

        [Fact]
        public void CannotLeaveIfNotCheckedIn()
        {
            Assert.Throws<InvalidOperationException>(() => lot.Leave("AB 123"));
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
