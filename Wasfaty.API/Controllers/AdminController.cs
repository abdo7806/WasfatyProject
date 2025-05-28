using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wasfaty.Application.DTOs.AdminDto;
using Wasfaty.Application.Interfaces;

namespace Wasfaty.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
        {
        
            var dashboardData = await _adminService.GetDashboardAsync();
            if (dashboardData == null)
            {
                return NotFound("Dashboard data not found.");
            }
            return Ok(dashboardData);
        }

    }
}
