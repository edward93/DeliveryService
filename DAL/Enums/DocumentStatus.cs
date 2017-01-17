using System.ComponentModel.DataAnnotations;

namespace DAL.Enums
{
    public enum DocumentStatus
    {
        [Display(Name = "Waiting For Approval")]
        WaitingForApproval = 0,
        Approved,
        Rejected
    }
}