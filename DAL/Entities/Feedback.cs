using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Feedback : Entity
    {
        [Required]
        public int Score { get; set; }
        public string Comment { get; set; }
        [ForeignKey("Driver")]
        public int DriverId { get; set; }
        [ForeignKey("Business")]
        public int BusinessId { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Business Business { get; set; }
    }
}