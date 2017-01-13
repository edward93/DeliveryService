using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;

namespace DAL.Entities
{
    public class Card : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        [ForeignKey("Business")]
        public int? BusinessId { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public CardType CardType { get; set; }
        [Required]
        public string CardHolderName { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
        [Required]
        public string SecurityCode { get; set; }
        [Required]
        public bool IsDefault { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Business Business { get; set; }

    }
}