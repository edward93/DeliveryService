using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DAL.Annotation;
using DAL.Entities;
using DAL.Enums;

namespace DeliveryService.ViewModels.Orders
{
    public class MakeOrderViewModel
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string OrderNumber { get; set; }
        [Required]
        public int TimeToReachPickUpLocation { get; set; }
        [Required]
        public int TimeToReachDropOffLocation { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public VehicleType VehicleType { get; set; }

        public string PickUpLocation { get; set; }
        public string PickUpName { get; set; }
        [Required, Precision(10, 6)]
        public decimal PickUpLatitude { get; set; }
        [Required, Precision(10, 6)]
        public decimal PickUpLongitude { get; set; }

        public string DropOffLocation { get; set; }
        public string DropOffName { get; set; }
        [Required, Precision(10, 6)]
        public decimal DropOffLatitude { get; set; }
        [Required, Precision(10, 6)]
        public decimal DropOffLongitude { get; set; }

        public GeoLocation GetPickUpLocation(DAL.Entities.Business business)
        {
            return new GeoLocation
            {
                Address = PickUpLocation,
                IsDeleted = false,
                CreatedDt = DateTime.UtcNow,
                Lat = PickUpLatitude,
                Long = PickUpLongitude,
                Name = PickUpName,
                UpdatedDt = DateTime.UtcNow,
                CreatedBy = business.ContactPerson.Id,
                UpdatedBy = business.ContactPerson.Id
            };
        }

        public GeoLocation GetDropOffLocation(DAL.Entities.Business business)
        {
            return new GeoLocation
            {
                Address = DropOffLocation,
                IsDeleted = false,
                CreatedDt = DateTime.UtcNow,
                Lat = DropOffLatitude,
                Long = DropOffLongitude,
                Name = DropOffName,
                UpdatedDt = DateTime.UtcNow,
                CreatedBy = business.ContactPerson.Id,
                UpdatedBy = business.ContactPerson.Id
            };
        }

        public Order GetOrder(DAL.Entities.Business business)
        {
            return new Order
            {
                BusinessId = business.Id,
                CreatedBy = business.ContactPerson.Id,
                CreatedDt = DateTime.UtcNow,
                CustomerName = CustomerName,
                CustomerPhone = CustomerPhone,
                IsDeleted = false,
                OrderNumber = OrderNumber,
                OrderStatus = OrderStatus.Pending,
                VehicleType = VehicleType,
                UpdatedBy = business.ContactPerson.Id,
                UpdatedDt = DateTime.UtcNow,
                TimeToReachDropOffLocation = TimeToReachDropOffLocation,
                TimeToReachPickUpLocation = TimeToReachPickUpLocation
            };
        }
    }
}