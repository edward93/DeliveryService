using System.ComponentModel.DataAnnotations;
using DAL.Enums;

namespace DAL.Entities
{
    public class Address : Entity
    {
        [Required]
        public int DriverId { get; set; }
        [Required]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Required]
        public Country Country { get; set; }
        [Required]
        public string City { get; set; }
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }

    }
}