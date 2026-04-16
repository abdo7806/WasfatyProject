using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.AdminDto;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.Application.Services
{
    public class AdminService : IAdminService
    {

        private readonly IAdminRepository _adminRepositorye;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepositorye = adminRepository;
          
        }
        public Task<AdminDashboardDto> GetDashboardAsync()
        {
            return _adminRepositorye.GetDashboardAsync();
            
        }
    }
}
