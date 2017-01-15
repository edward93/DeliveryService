namespace DAL.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
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