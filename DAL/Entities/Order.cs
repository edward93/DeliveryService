using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Annotation;
using DAL.Entities.Interfaces;
using DAL.Enums;

namespace DAL.Entities
{
    public class Order : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string OrderNumber { get; set; }
        [Required]
        [Description("Time to reach pick up locatoin in mins")]
        public int TimeToReachPickUpLocation { get; set; }
        [Required]
        [Description("Time to reach drop off locatoin in mins")]
        public int TimeToReachDropOffLocation { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public VehicleType VehicleType { get; set; }
        [ForeignKey("AssignedDriver")]
        public int? AssignedDriverId { get; set; }
        [Required]
        [ForeignKey("Business")]
        public int BusinessId { get; set; }

        [ForeignKey("PickUpLocation")]
        public int PickUpLocationId { get; set; }
        [ForeignKey("DropOffLocation")]
        public int DropOffLocationId { get; set; }

        [Precision(19, 4), Required]
        public decimal OrderPrice { get; set; }


        public virtual Driver AssignedDriver { get; set; }
        public virtual Business Business { get; set; }

        public virtual GeoLocation PickUpLocation { get; set; }
        public virtual GeoLocation DropOffLocation { get; set; }
        public virtual IEnumerable<DriverFee> DriverFees { get; set; }
        public virtual IEnumerable<DriverPenalty> DriverPenalties { get; set; }
        public virtual IEnumerable<BusinessPenalty> BusinessPenalties { get; set; }
    }
}