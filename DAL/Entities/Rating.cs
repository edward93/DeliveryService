using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Rating : Entity
    {
        [Key, ForeignKey("Driver")]
        public new int Id { get; set; }
        [Required]
        public decimal AverageScore { get; set; }

        public virtual Driver Driver { get; set; }
    }
}