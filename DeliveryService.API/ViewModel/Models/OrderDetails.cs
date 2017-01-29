using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using DAL.Annotation;
using DAL.Entities;
using DAL.Enums;

namespace DeliveryService.API.ViewModel.Models
{
    public class OrderDetails
    {
        public OrderDetails(Order order)
        {
            Id = order.Id;
            CustomerName = order.CustomerName;
            CustomerPhone = order.CustomerPhone;
            OrderNumber = order.OrderNumber;
            TimeToReachDropOffLocation = order.TimeToReachDropOffLocation;
            TimeToReachPickUpLocation = order.TimeToReachPickUpLocation;
            OrderStatus = order.OrderStatus;
            PickUpLocationAddress = order.PickUpLocation.Address;
            PickUpLocationLat = (double)order.PickUpLocation.Lat;
            PickUpLocationLong = (double)order.PickUpLocation.Long;
            DropOffLocationAddress = order.DropOffLocation.Address;
            DropOffLocationLat = (double)order.DropOffLocation.Lat;
            DropOffLocationLong = (double)order.DropOffLocation.Long;
            OrderPrice = order.OrderPrice;
            NotDeliveredReason = order.NotDeliveredReason;
            BusinessId = order.BusinessId;
            BusinessName = order.Business.BusinessName;
        }
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string OrderNumber { get; set; }
        public int TimeToReachPickUpLocation { get; set; }
        public int TimeToReachDropOffLocation { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public int BusinessId { get; set; }
        public string BusinessName { get; set; }

        public string PickUpLocationAddress { get; set; }
        public double PickUpLocationLong { get; set; }
        public double PickUpLocationLat { get; set; }

        public string DropOffLocationAddress { get; set; }
        public double DropOffLocationLong { get; set; }
        public double DropOffLocationLat { get; set; }

        public decimal OrderPrice { get; set; }
        public string NotDeliveredReason { get; set; }
    }
}