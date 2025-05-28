using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.AdminDto;

namespace Wasfaty.Infrastructure.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<AdminDashboardDto> GetDashboardAsync();

    }
}
