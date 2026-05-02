using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.PatientAuthorization.Requirements;
using Wasfaty.Application.DTOs.Patients;

namespace Wasfaty.API.Authorization.PatientAuthorization.Handlers
{
    public class CanAccessPatientHandler : AuthorizationHandler<CanAccessPatientRequirement, PatientDto>
    {
        protected override Task HandleRequirementAsync(
             AuthorizationHandlerContext context,
             CanAccessPatientRequirement requirement,
             PatientDto patient)
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            // Admin يقدر يشوف الكل
            if (role == "Admin")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Doctor يقدر يشوف المرضى
            if (role == "Doctor")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // المريض يشوف نفسه فقط
            if (role == "Patient" && patient.UserId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    
    }
}
