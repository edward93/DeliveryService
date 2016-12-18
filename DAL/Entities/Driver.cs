using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;

namespace DAL.Entities
{
    public class Driver : Entity
    {
        [Key, ForeignKey("Person")]
        public new int Id { get; set; }
        [Required]
        public VehicleType VehicleType { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        [Required]
        public bool Approved { get; set; }
        [Required]
        public DriverStatus Status { get; set; }

        public virtual Rating Rating { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }


    }
}