using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DAL.Annotation;

namespace DeliveryService.API.ViewModel.Models
{
    public class DriverLocationModel
    {
        public int DriverId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Precision(10, 6)]
        public decimal Long { get; set; }
        [Required]
        [Precision(10, 6)]
        public decimal Lat { get; set; }
    }
}