using System.ComponentModel.DataAnnotations;

namespace DeliveryService.Models.ViewModels
{
    public class AcceptRejectDriverViewModel
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int DriverId { get; set; }
    }
}