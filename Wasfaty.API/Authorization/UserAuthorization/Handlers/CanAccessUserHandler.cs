using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.PrescriptionAuthorization.Requirements;
using Wasfaty.API.Authorization.UserAuthorization.Requirements;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.API.Authorization.UserAuthorization.Handlers
{
    public class CanAccessUserHandler : AuthorizationHandler<CanAccessUserRequirement, UserDto>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanAccessUserRequirement requirement,
            UserDto resource)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || resource == null)
                return Task.CompletedTask;

            var currentUserId = int.Parse(userIdClaim);

            // Admin يقدر يشوف أي مستخدم
            if (role == "Admin")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // المستخدم يشوف نفسه فقط
            if (resource.Id == currentUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
