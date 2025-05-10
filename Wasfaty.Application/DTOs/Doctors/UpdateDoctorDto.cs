using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Doctors
{
    public class UpdateDoctorDto
    {
        public int MedicalCenterId { get; set; }
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
    }
}
