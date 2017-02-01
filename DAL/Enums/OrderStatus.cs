using System.ComponentModel.DataAnnotations;

namespace DAL.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        [Display(Name = "Driver Accepted By Business")]
        DriverAcceptedByBusiness,
        [Display(Name = "Accepted By Driver")]
        AcceptedByDriver,
        [Display(Name = "Rejected By Driver")]
        RejectedByDriver,
        [Display(Name = "On The Way To Pick Up")]
        OnTheWayToPickUp,
        [Display(Name = "Arrived At The Pick Up Location")]
        ArrivedAtThePickUpLocation,
        [Display(Name = "Order Picked Up")]
        OrderPickedUp,
        Delivered,
        [Display(Name = "Not Delivered")]
        NotDelivered,
        [Display(Name = "Return Booked")]
        ReturnBooked,
        [Display(Name = "Return Canceled")]
        ReturnCanceled,
        [Display(Name = "Return Confirmed")]
        ReturnConfirmed
    }
}