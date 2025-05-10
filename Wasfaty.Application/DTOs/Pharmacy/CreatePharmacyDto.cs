using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Pharmacies
{
    public class CreatePharmacyDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(300, ErrorMessage = "Address can't be longer than 300 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
    }
}
