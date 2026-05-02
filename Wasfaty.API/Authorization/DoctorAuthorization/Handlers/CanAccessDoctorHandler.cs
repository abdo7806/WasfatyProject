using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.DoctorAuthorization.Requirements;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Doctors;

namespace Wasfaty.API.Authorization.DoctorAuthorization.Handlers
{
    public class CanAccessDoctorHandler : AuthorizationHandler<CanAccessDoctorRequirement, DoctorDto>
    {
        protected override Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      CanAccessDoctorRequirement requirement,
      DoctorDto resource)
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == Roles.Admin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (role == Roles.Doctor && resource.UserId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
