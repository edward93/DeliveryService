namespace DAL.Enums
{
    public enum PaymentType
    {
        OrderBookingFee = 0,
        AotmBox,
        DriverFeeFor3Miles,
        DriverPenaltyForDelayPerMinute,
        DriverPenaltyForRejections,
        BikeOrScooterExtraMileage,
        CarOrVanExtraMileage,
        DriverWaitingForBusinessPerMinute,
        BusinessRejectsDriver,
        Other
    }
}