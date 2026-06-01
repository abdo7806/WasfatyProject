using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Pharmacists
{
    public class CreatePharmacistDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int PharmacyId { get; set; } 

        [Required]
        [StringLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;
    }
}
