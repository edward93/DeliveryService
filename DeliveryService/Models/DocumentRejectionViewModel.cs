using System.ComponentModel.DataAnnotations;

namespace DeliveryService.Models
{
    public class DocumentRejectionViewModel
    {
        [Required]
        public int DocumentId { get; set; }
        [Required]
        public string RejectionComment { get; set; }
    }
}