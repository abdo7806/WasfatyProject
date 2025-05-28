using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.AdminDto;
using Wasfaty.Application.Interfaces;
using Wasfaty.Infrastructure.Repositories;
using Wasfaty.Infrastructure.Repositories.Interfaces;

namespace Wasfaty.Infrastructure.Services
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
