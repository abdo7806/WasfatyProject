using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Pharmacists
{
    public class UpdatePharmacistDto
    {
        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public int PharmacyId { get; set; }
    }
}
