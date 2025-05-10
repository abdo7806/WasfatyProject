using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Doctors
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MedicalCenterId { get; set; }
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }

        public MedicalCenterDto MedicalCenter { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}
