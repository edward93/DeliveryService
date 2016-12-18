using System.ComponentModel.DataAnnotations;

namespace DAL.Enums
{
    public enum VehicleType
    {
        Sedan= 0,
        Coup,
        [Display(Name = "SUV")]
        Suv,
        Bicycle,
        Other

    }
}