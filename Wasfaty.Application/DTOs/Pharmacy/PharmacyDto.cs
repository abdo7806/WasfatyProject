using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Pharmacists;

namespace Wasfaty.Application.DTOs.Pharmacies
{
    public class PharmacyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public List<DispenseRecordDto> DispenseRecords { get; set; } = new List<DispenseRecordDto>();
        public List<PharmacistDto> Pharmacists { get; set; } = new List<PharmacistDto>();

    }
}
