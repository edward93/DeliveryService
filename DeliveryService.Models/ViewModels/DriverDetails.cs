using System;
using System.Runtime.Serialization;
using DAL.Annotation;
using DAL.Entities;

namespace DeliveryService.Models.ViewModels
{
    public class DriverDetails
    {
        public DriverDetails()
        {
        }
        public DriverDetails(Order order, Driver driver)
        {
            DriverId = driver.Id;
            OrderId = order.Id;
            DriverLong = driver.DriverLocation.Long;
            DriverLat = driver.DriverLocation.Lat;
            OrderPickUpLong = order.PickUpLocation.Long;
            OrderPickUpLat = order.PickUpLocation.Lat;
            Rating = driver.Rating.AverageScore;
            BusinessId = order.BusinessId;
        }

        public int DriverId { get; set; }
        public int OrderId { get; set; }
        public int BusinessId { get; set; }
        [Precision(10, 6)]
        public decimal DriverLong { get; set; }
        [Precision(10, 6)]
        public decimal DriverLat { get; set; }
        [Precision(10, 6)]
        public decimal OrderPickUpLong { get; set; }
        [Precision(10, 6)]
        public decimal OrderPickUpLat { get; set; }
        public decimal Rating { get; set; }
    }
}