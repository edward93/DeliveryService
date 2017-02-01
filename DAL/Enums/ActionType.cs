namespace DAL.Enums
{
    public enum ActionType
    {
        OrderCreated = 0,
        DriverAcceptedByBusiness,
        DriverCanceledByBusiness,
        DriverAcceptedOrder,
        DriverRejectedOrder,
        DriverIsOnTheWayToPickUp,
        DriverArrivedAtPickUpLocation,
        DriverPickedUpTheOrder,
        DriverArrived,
        OrderDelivered,
        Other
    }
}