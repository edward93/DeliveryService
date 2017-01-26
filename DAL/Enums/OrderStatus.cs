﻿namespace DAL.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        DriverAcceptedByBusiness,
        AcceptedByDriver,
        RejectedByDriver,
        OnTheWayToPickUp,
        ArrivedAtThePickUpLocation,
        OrderPickedUp,
        Delivered,
        NotDelivered,
        ReturnBooked,
        ReturnCanceled,
        ReturnConfirmed
    }
}