using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Annotation;
using DAL.Entities.Interfaces;

namespace DAL.Entities
{
    public class Discount : IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Precision(19,4), Required]
        public decimal DiscountAmount { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }

        [ForeignKey("Rate")]
        public int RateId { get; set; }
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        [ForeignKey("Business")]
        public int? BusinessId { get; set; }

        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Rate Rate { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual Business Business { get; set; }
    }
}