namespace DAL.Enums
{
    // TODO: Complete this list
    public enum ActionType
    {
        OrderCreated = 0,
        DriverAcceptedByBusiness,
        DriverCanceledByBusiness,
        DriverAcceptedOrder,
        DriverRejectedOrder,
        DriverIsOnTheWayToPickUp,
        DriverArrived,
        Other
    }
}