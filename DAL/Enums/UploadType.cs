using System.ComponentModel.DataAnnotations;

namespace DAL.Enums
{
    public enum UploadType
    {
        Passport = 0,
        Insurance = 1,
        [Display(Name = "Proof of Address")]
        ProofOfAddress = 2,
        Photo = 3,
        License = 4,
        Other = 5
    }
}