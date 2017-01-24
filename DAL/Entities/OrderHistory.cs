using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Annotation;
using DAL.Entities.Interfaces;
using DAL.Enums;

namespace DAL.Entities
{
    public class OrderHistory : IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        [Required]
        public ActionType Action { get; set; }
        [Precision(19, 4), Required]
        public decimal OrderPrice { get; set; }
        [Required]
        public int TimeToReachPickUpLocation { get; set; }
        [Required]
        public int TimeToReachDropOffLocation { get; set; }
        public int? ActuallTimeToPickUpLocation { get; set; }
        public int? ActualTimeToDropOffLocation { get; set; }
        [ForeignKey("DriverFee")]
        public int? DriverFeeId { get; set; }
        [ForeignKey("DriverPenalty")]
        public int? DriverPenaltyId { get; set; }
        [ForeignKey("BusinessPenalty")]
        public int? BusinessPenaltyId { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Order Order { get; set; }

        public virtual DriverFee DriverFee { get; set; }
        public virtual DriverPenalty DriverPenalty { get; set; }
        public virtual BusinessPenalty BusinessPenalty { get; set; }
    }
}