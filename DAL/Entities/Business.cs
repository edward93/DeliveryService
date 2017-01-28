using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Interfaces;

namespace DAL.Entities
{
    public class Business : IEntity
    {
        [Key]
        [ForeignKey("ContactPerson")]
        public int Id { get; set; }
        [Required]
        public string BusinessName { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        [Required]
        public bool Approved { get; set; }

        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("Rating")]
        public int? RatingId { get; set; }

        public virtual Rating Rating { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual Person ContactPerson { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Discount> Discounts { get; set; }
    }
}