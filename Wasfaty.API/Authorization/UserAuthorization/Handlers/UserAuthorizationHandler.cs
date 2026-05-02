using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.UserAuthorization.Requirements;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.API.Authorization.UserAuthorization.Handlers
{
    public class CanEditUserHandler : AuthorizationHandler<CanEditUserRequirement, UserDto>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanEditUserRequirement requirement,
            UserDto resource)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || resource == null)
                return Task.CompletedTask;

            var currentUserId = int.Parse(userIdClaim);

            // Admin يقدر يعدل أي مستخدم
            if (role == "Admin")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // المستخدم يعدل نفسه فقط
            if (resource.Id == currentUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
