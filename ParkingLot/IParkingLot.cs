using System;
using System.Collections.Generic;

namespace ParkingLot
{
    public interface IParkingLot
    {
        void Checkin(string licensePlate);

        void BeginCheckout(string licensePlate);

        decimal GetRemainingFee(string licensePlate);
        void Pay(string licensePlate, decimal amount);
        void Leave(string licensePlate);
    }
}
