using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.AdminDto;
using Wasfaty.Infrastructure.Repositories.Interfaces;

namespace Wasfaty.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalDoctors = await _context.Doctors.CountAsync();
                var totalPatients = await _context.Patients.CountAsync();
                var totalPharmacists = await _context.Pharmacists.CountAsync();
                var totalPharmacies = await _context.Pharmacies.CountAsync();
                var totalPrescriptions = await _context.Prescriptions.CountAsync();
                var totalDispensed = await _context.DispenseRecords.CountAsync();
                var totalMedications = await _context.Medications.CountAsync();
                var totaPendingPrescriptions = await _context.Prescriptions.Where(p => !p.IsDispensed).CountAsync();

                return new AdminDashboardDto
                {
                    TotalUsers = totalUsers,
                    TotalDoctors = totalDoctors,
                    TotalPatients = totalPatients,
                    TotalPharmacists = totalPharmacists,
                    TotalPharmacies = totalPharmacies,
                    TotalPrescriptions = totalPrescriptions,
                    TotalDispensedPrescriptions = totalDispensed,
                    TotalPendingPrescriptions = totaPendingPrescriptions,
                    TotalMedications = totalMedications
                };
            }
            catch (Exception ex)
            {
                // يمكنك تسجيل الخطأ هنا باستخدام نظام تسجيل الأخطاء الذي تستخدمه
               // throw new Exception("An error occurred while retrieving the dashboard data.", ex);
                return null; // في حالة حدوث خطأ، يمكنك إرجاع null أو معالجة الخطأ بطريقة أخرى حسب الحاجة
            }
        }
    }
}
