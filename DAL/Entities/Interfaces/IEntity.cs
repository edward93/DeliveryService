using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities.Interfaces
{
    public interface IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        int Id { get; set; }
        DateTime CreatedDt { get; set; }
        int CreatedBy { get; set; }
        DateTime UpdatedDt { get; set; }
        int UpdatedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}