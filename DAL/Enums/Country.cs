using System.ComponentModel.DataAnnotations;

namespace DAL.Enums
{
    public enum Country
    {
        [Display(Name = "United Kingdom")]
        Uk = 0,
        [Display(Name = "United States")]
        UnitedStates = 1,
        Armenia,
        Other
    }
}