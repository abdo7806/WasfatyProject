using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Application.DTOs.Pharmacists
{

    public class PharmacistDashboardStatsDto
    {
        public int PendingPrescriptions { get; set; }
        public int DispensedPrescriptionsByPharmacy { get; set; }//الوصفات الطبية التي يتم صرفها من قبل الصيدلية

        public int DispensedPrescriptionsByPharmcist { get; set; }//الوصفات الطبية التي تم صرفها بواسطة الصيدلية
        public int MonthlyMedications { get; set; }//الأدوية الشهرية
        public List<TopMedicationDTO> TopMedications { get; set; }
    }

    public class TopMedicationDTO
    {
        public string MedicationName { get; set; }
        public int Count { get; set; }
    }
}

 

    

