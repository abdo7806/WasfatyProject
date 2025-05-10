using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Medications
{
    public class UpdateMedicationDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Dosage Form can't be longer than 100 characters.")]
        public string? DosageForm { get; set; }

        [StringLength(100, ErrorMessage = "Strength can't be longer than 100 characters.")]
        public string? Strength { get; set; }

       // [Required]
       // [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
       // public decimal Price { get; set; }
    }
}
