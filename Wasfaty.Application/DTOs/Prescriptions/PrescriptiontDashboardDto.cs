using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Application.DTOs.Prescriptions
{
    public class PrescriptiontDashboardDto
    {
        public int DoctorCount { get; set; }
        public int PharmacistCount { get; set; }
        public int PatientCount { get; set; }
        public int PrescriptiontCount { get; set; }
    }
}
