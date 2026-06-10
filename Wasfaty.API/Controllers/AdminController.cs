using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.AdminDto;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.API.Controllers
{
    [Authorize(Roles = Roles.Admin)]
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
        [EnableRateLimiting("DashboardPolicy")]
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
