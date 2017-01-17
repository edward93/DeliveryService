using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using DAL.Annotation;
using DAL.Entities.Interfaces;

namespace DAL.Entities
{
    public class GeoLocation :IEntity, IGeoLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required, Precision(10,6)]
        public decimal Long { get; set; }
        [Required, Precision(10, 6)]
        public decimal Lat { get; set; }
    }
}