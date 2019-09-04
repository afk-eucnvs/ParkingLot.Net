using System;
using System.Collections.Generic;

namespace ParkingLot
{
    public class ParkingLot : IParkingLot
    {
        readonly Dictionary<string, DateTime> checkedInCars = new Dictionary<string, DateTime>();
        readonly Dictionary<string, decimal> debt = new Dictionary<string, decimal>();
        readonly IClock clock;
        readonly TimeSpan span;
        readonly decimal price;
        readonly TimeSpan freeTime;

        public ParkingLot(IClock clock, decimal price, TimeSpan span, TimeSpan freeTime)
        {
            this.clock = clock;
            this.span = span;
            this.price = price;
            this.freeTime = freeTime;
        }

        public void Checkin(string licensePlate)
        {
            if (checkedInCars.ContainsKey(licensePlate))
            {
                Error("Car already checked in");
            }
            checkedInCars.Add(licensePlate, clock.Now());
            Gates.OpenEntranceGate();
        }

        public void BeginCheckout(string licensePlate)
        {
            if (checkedInCars.ContainsKey(licensePlate))
            {
                var checkinTime = checkedInCars[licensePlate];
                var checkoutTime = clock.Now();
                var time = checkoutTime - checkinTime;

                var payTime = (time - freeTime);

                if (payTime < TimeSpan.Zero)
                {
                    debt.Add(licensePlate, 0);
                }
                else
                {

                var timeIntervalsParked = (decimal)(payTime / span);
                var intervalsBegun = Math.Ceiling(timeIntervalsParked);

                debt.Add(licensePlate, intervalsBegun * price);
                }
                checkedInCars.Remove(licensePlate);
            }
            else
            {
                Error("Car not checkedIn");
            }
        }

        public decimal GetRemainingFee(string licensePlate)
        {
            return debt.GetValueOrDefault(licensePlate, 0);
        }

        public void Pay(string licensePlate, decimal amount)
        {
            if (debt.ContainsKey(licensePlate))
            {
                if (amount >= 0)
                {
                    debt[licensePlate] -= amount;
                }
                else
                {
                    Error("You did not pay!");
                }
            }
            else
            {
                Error("Something went wrong");
            }
        }

        public void Leave(string licensePlate)
        {
            if (debt.ContainsKey(licensePlate) && debt[licensePlate] <= 0)
            {
                debt.Remove(licensePlate);
                checkedInCars.Remove(licensePlate);
                Gates.OpenExitGate();
            }
            else
            {
                Error("Please pay!");
            }
        }

        void Error(string msg)
        {
            throw new InvalidOperationException(msg);
        }
    }
}
