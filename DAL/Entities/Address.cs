using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Interfaces;
using DAL.Enums;

namespace DAL.Entities
{
    public class Address : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        [ForeignKey("Business")]
        public int? BusinessId { get; set; }
        [Required]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Required]
        public Country Country { get; set; }
        [Required]
        public string City { get; set; }
        public string State { get; set; }
        [Required, StringLength(10)]
        public string ZipCode { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Business Business { get; set; }

    }
}