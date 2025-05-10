using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Pharmacists;
using Wasfaty.Application.DTOs.Prescriptions;

namespace Wasfaty.Application.DTOs.DispenseRecords
{
    public class DispenseRecordDto
    {
        public int Id { get; set; }

        public int PrescriptionId { get; set; }

        public int PharmacistId { get; set; }

        public int PharmacyId { get; set; }

        public DateTime DispensedDate { get; set; }

       // public string PharmacistName { get; set; } = string.Empty; // إضافة اسم الصيدلي

       // public string PharmacyName { get; set; } = string.Empty; // إضافة اسم الصيدلية

        public virtual PharmacistDto Pharmacist { get; set; } = null!;

        public virtual PharmacyDto Pharmacy { get; set; } = null!;

        public virtual PrescriptionDto Prescription { get; set; } = null!;

        //  public DateTime PrescriptionDate { get; set; } // إضافة تاريخ الوصفة
    }
}
