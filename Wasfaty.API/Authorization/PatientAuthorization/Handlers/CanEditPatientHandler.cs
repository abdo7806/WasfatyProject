using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.PatientAuthorization.Requirements;
using Wasfaty.Application.DTOs.Patients;

namespace Wasfaty.API.Authorization.PatientAuthorization.Handlers
{
    public class CanEditPatientHandler : AuthorizationHandler<CanEditPatientRequirement, PatientDto>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            CanEditPatientRequirement requirement,
            PatientDto patient)
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Admin")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (role == "Patient" && patient.UserId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
