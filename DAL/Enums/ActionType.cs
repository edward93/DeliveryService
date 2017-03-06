namespace DAL.Enums
{
    public enum ActionType
    {
        OrderCreated = 0,
        DriverAcceptedByBusiness,
        DriverRejectedByBusiness,
        DriverAcceptedOrder,
        DriverRejectedOrder,
        DriverIsOnTheWayToPickUp,
        DriverArrivedAtPickUpLocation,
        DriverPickedUpTheOrder,
        DriverArrived,
        OrderDelivered,
        OrderNotDelivered,
        Other
    }
}