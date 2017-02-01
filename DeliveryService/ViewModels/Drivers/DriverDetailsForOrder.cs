using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Enums;

namespace DeliveryService.ViewModels.Drivers
{
    public class DriverDetailsForOrder
    {
        public string FullName { get; set; }
        public string VehicleType { get; set; }

        public string VehicleRegNumber { get; set; }
        public decimal RatingAverageScore { get; set; }
    }
}