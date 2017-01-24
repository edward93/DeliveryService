namespace DAL.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        DriverAcceptedByBusiness,
        AcceptedByDriver,
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