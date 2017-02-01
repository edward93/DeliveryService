using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryService.API.Hubs
{
    public class DriverHubModel
    {
        public int DriverId { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
    }
}