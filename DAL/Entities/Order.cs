using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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


        public virtual Driver AssignedDriver { get; set; }
        public virtual Business Business { get; set; }

        public GeoLocation PickUpLocation { get; set; }
        public GeoLocation DropOffLocation { get; set; }
    }
}