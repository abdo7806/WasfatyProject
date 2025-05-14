using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Prescriptions;

namespace Wasfaty.Application.DTOs.Patients
{
    public class PatientDashboardDto
    {
        public int TotalPrescriptions { get; set; }
        public int DispensedMeds { get; set; }
        public PrescriptionDto? LatestPrescription { get; set; }
    }
}
