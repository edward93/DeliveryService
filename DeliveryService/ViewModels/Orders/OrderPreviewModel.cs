using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAL.Annotation;
using DAL.Enums;

namespace DeliveryService.ViewModels.Orders
{
    public class OrderPreviewModel
    {
        public OrderPreviewModel(Order order)
        {
            OrderPrice = order.OrderPrice;
            CustomerName = order.CustomerName;
            CustomerPhone = order.CustomerPhone;
            OrderNumber = order.OrderNumber;
            TimeToReachDropOffLocation = order.TimeToReachDropOffLocation;
            TimeToReachPickUpLocation = order.TimeToReachPickUpLocation;
            OrderStatus = order.OrderStatus;
            VehicleType = order.VehicleType;

            PickUpLatitude = order.PickUpLocation.Lat;
            PickUpLongitude = order.PickUpLocation.Long;
            PickUpLocation = order.PickUpLocation.Address;

            DropOffLocation = order.DropOffLocation.Address;
            DropOffLatitude = order.DropOffLocation.Lat;
            DropOffLongitude = order.DropOffLocation.Long;
        }


        public decimal OrderPrice { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string OrderNumber { get; set; }
        public int TimeToReachPickUpLocation { get; set; }
        public int TimeToReachDropOffLocation { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public VehicleType VehicleType { get; set; }


        public string PickUpLocation { get; set; }
        public string PickUpName { get; set; }
        public decimal PickUpLatitude { get; set; }
        public decimal PickUpLongitude { get; set; }

        public string DropOffLocation { get; set; }
        public string DropOffName { get; set; }
        public decimal DropOffLatitude { get; set; }
        public decimal DropOffLongitude { get; set; }
    }
}