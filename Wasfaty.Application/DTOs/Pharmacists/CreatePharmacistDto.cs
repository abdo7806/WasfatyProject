using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Pharmacists
{
    public class CreatePharmacistDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int PharmacyId { get; set; } 

        [Required]
        [StringLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;
    }
}
