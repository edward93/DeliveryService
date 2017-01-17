using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Interfaces;

namespace DAL.Entities
{
    public class DeviceOrientation : IEntity
    {
        [Key]
        [ForeignKey("Driver")]
        public int Id { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        [Range(0, 360, ErrorMessage = "Can only be between 0 .. 360")]
        public int OrientationYaw { get; set; }

        public Driver Driver { get; set; }
    }
}