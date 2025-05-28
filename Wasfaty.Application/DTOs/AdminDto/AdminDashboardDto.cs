using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Application.DTOs.AdminDto
{
    public class AdminDashboardDto
    {
        public int TotalPrescriptions { get; set; } // عدد الوصفات الطبية
        public int TotalPharmacies { get; set; } // عدد الصيدليات
        public int TotalPharmacists { get; set; } // عدد الصيادلة
        public int TotalPatients { get; set; } // عدد المرضى
        public int TotalDispensedPrescriptions { get; set; } // عدد سجلات الصرف
        public int TotalUsers { get; set; } // عدد المستخدمين
        public int TotalPendingPrescriptions { get; set; } // عدد الوصفات الطبية المعلقة
        public int TotalDoctors { get; set; } // عدد الأطباء
        public int TotalMedications { get; set; } // عدد الادويه
    }
}
