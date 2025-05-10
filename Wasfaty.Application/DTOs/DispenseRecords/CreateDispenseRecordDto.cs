namespace Wasfaty.Application.DTOs.DispenseRecords
{
    public class CreateDispenseRecordDto
    {
        public int PrescriptionId { get; set; }

        public int PharmacistId { get; set; }

        public int PharmacyId { get; set; }

        public DateTime DispensedDate { get; set; }
    }
}
