using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities;

namespace DAL.Enums
{
    public class Address : Entity
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public Country Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        [ForeignKey("Person")]
        public int PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}