using System.ComponentModel.DataAnnotations;

namespace DAL.Enums
{
    public enum RiderStatus
    {
        Online = 0,
        Offline,
        Busy,
        [Display(Name = "No GPS")]
        NoGps,
        [Display(Name = "Disconnected From Hub")]
        DisconnectedFromHub,
        Other
    }
}