using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliveryService.API.ViewModel.Enums;

namespace DeliveryService.API.ViewModel.Models
{
    public class DriverDocumentModel
    {
        public int DriverId { get; set; }
        public DriverDocumentTypeEnum DocumentType { get; set; }

    }
}