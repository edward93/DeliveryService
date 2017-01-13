using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public virtual ICollection<Card> Cards { get; set; }
        public virtual Person ContactPerson { get; set; }

    }
}