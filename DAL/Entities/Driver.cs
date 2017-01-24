using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Interfaces;
using DAL.Enums;

namespace DAL.Entities
{
    public class Driver : IEntity
    {
        [Key, ForeignKey("Person")]
        public int Id { get; set; }
        
        [Required]
        public VehicleType VehicleType { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public bool HasAotmBox { get; set; }
        [Required]
        public bool Approved { get; set; }
        [Required]
        public DriverStatus Status { get; set; }
        [ForeignKey("Rating")]
        public int? RatingId { get; set; }

        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }


        public virtual Rating Rating { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Discount> Discounts { get; set; }

    }
}