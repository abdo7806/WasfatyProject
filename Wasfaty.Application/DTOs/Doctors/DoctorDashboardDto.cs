using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Application.DTOs.Doctors
{
    public class DoctorDashboardDto
    {
        public int TotalPrescriptions { get; set; }// عدد الوصافت الطبيه الذي صرفها الطبيب
        public int DispensedPrescriptions { get; set; }// عدد الوصفات المصروفه التابعه للطبيب
        public int PendingPrescriptions { get; set; }// عدد الوصفات غير المصروفه التابعه للطبيب
        public int UniquePatients { get; set; }// 
    }
}
