using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;

namespace DAL.Entities
{
    public class Card : Entity
    {
        [ForeignKey("Driver")]
        [Required]
        public int DriverId { get; set; }
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

        public virtual Driver Driver{ get; set; }
    }
}