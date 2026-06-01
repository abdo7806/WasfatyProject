using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Pharmacists
{
    public class UpdatePharmacistDto
    {

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public int PharmacyId { get; set; }
    }
}
