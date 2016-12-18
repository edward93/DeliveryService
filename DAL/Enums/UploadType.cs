using System.ComponentModel.DataAnnotations;

namespace DAL.Enums
{
    public enum UploadType
    {
        Passport = 0,
        Insurance,
        [Display(Name = "Proof of Address")]
        ProofOfAddress,
        Photo,
        Other
    }
}