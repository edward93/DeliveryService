using DAL.Annotation;
using DAL.Entities;
using DAL.Enums;
using Infrastructure.Helpers;

namespace DeliveryService.Models.ViewModels
{
    public class DriverDetailsWithLocation
    {
        public DriverDetailsWithLocation()
        {
        }

        public DriverDetailsWithLocation(Driver driver, Order order, int? businessId = null)
        {
            DriverId = driver.Id;
            DriveFullName = $"{driver.Person.FirstName} {driver.Person.LastName}";
            VehicleType = EnumHelpers<VehicleType>.GetDisplayValue(driver.VehicleType);
            DriverRating = driver.Rating.AverageScore;
            DriverLong = driver.DriverLocation.Long;
            DriverLat = driver.DriverLocation.Lat;
            DriverStatus = EnumHelpers<DriverStatus>.GetDisplayValue(driver.Status);

            if (businessId != null) BusinessId = businessId.Value;

            if (order == null) return;

            OrderStatus = EnumHelpers<OrderStatus>.GetDisplayValue(order.OrderStatus);
            OrderId = order.Id;
            BusinessId = order.BusinessId;
        }

        public int DriverId { get; set; }
        public int? OrderId { get; set; }
        public int BusinessId { get; set; }
        public string OrderStatus { get; set; }
        public string DriveFullName { get; set; }
        public string DriverStatus { get; set; }
        public decimal DriverRating { get; set; }
        public string VehicleType { get; set; }
        [Precision(10, 6)]
        public decimal DriverLong { get; set; }
        [Precision(10, 6)]
        public decimal DriverLat { get; set; }
    }
}