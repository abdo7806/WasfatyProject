using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Pharmacists
{
    public class PharmacistDto
    {
        public int Id { get; set; }
        public int UserId { get; set; } // مرتبط بـ User
        public string LicenseNumber { get; set; } = string.Empty;
        public string PharmacyName { get; set; } = string.Empty;
        public int PharmacyId { get; set; } // إذا كان مرتبطًا بالصيدلية

        public List<DispenseRecordDto> DispenseRecords { get; set; } = new List<DispenseRecordDto>();
        public PharmacyDto Pharmacy { get; set; } = null!;
        public UserDto User { get; set; } = null!;

    }
}
