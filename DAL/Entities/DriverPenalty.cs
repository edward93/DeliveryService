using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Annotation;
using DAL.Entities.Interfaces;
using DAL.Enums;

namespace DAL.Entities
{
    public class DriverPenalty : IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        [Precision(19, 4)]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public string Description { get; set; }
        [Required]
        public PenaltyType PenaltyType { get; set; }

        [ForeignKey("Driver")]
        public int DriverId { get; set; }

        [ForeignKey("Order")]
        public int? AssociatedOrderId { get; set; }

        public virtual Order Order { get; set; }
        public virtual Driver Driver { get; set; }
    }
}