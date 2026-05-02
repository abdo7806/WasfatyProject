using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.PharmacistAuthorization.Requirements;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.Pharmacists;
using Wasfaty.Application.DTOs.Pharmacists;

namespace Wasfaty.API.Authorization.PharmacistAuthorization.Handlers
{
    public class CanAccessPharmacistHandler : AuthorizationHandler<CanAccessPharmacistRequirement, PharmacistDto>
    {
        protected override Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      CanAccessPharmacistRequirement requirement,
      PharmacistDto resource)
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == Roles.Admin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (role == Roles.Pharmacist && resource.UserId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
