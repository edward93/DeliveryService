using System.ComponentModel.DataAnnotations;

namespace DAL.Enums
{
    public enum CardType
    {
        Visa = 0,
        [Display(Name = "Master Card")]
        MasterCard,
        [Display(Name = "American Express")]
        AmericanExpress,
        Other
    }
}