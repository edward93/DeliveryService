using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;

namespace DAL.Entities
{
    public class Person : IEntity
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
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public Sex Sex { get; set; }
        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual User User { get; set; }
        public virtual Driver Driver { get; set; }
        
    }
}