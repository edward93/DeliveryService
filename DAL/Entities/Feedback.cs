using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Feedback : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }

        [Required]
        public int Score { get; set; }
        public string Comment { get; set; }
        [ForeignKey("Driver")]
        public int DriverId { get; set; }
        [ForeignKey("Business")]
        public int BusinessId { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Business Business { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}