using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using DAL.Annotation;
using DAL.Entities;
using DAL.Enums;

namespace DeliveryService.ViewModels.Business
{
    public class BusinessOrder
    {
        public BusinessOrder(Order order)
        {
            Id = order.Id;
            CustomerPhone = order.CustomerPhone;
            CustomerName = order.CustomerName;
            OrderNumber = order.OrderNumber;
            TimeToReachDropOffLocation = order.TimeToReachDropOffLocation;
            TimeToReachPickUpLocation = order.TimeToReachPickUpLocation;
            OrderStatus = order.OrderStatus.ToString();
            VehicleType = order.VehicleType.ToString();
        }

        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string OrderNumber { get; set; }
        public int TimeToReachPickUpLocation { get; set; }
        public int TimeToReachDropOffLocation { get; set; }
        public string OrderStatus { get; set; }
        public string VehicleType { get; set; }
    }
}