using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.PrescriptionAuthorization.Requirements;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.API.Authorization.PrescriptionAuthorization.Handlers
{
    public class CanDispensePrescriptionHandler
        : AuthorizationHandler<CanDispensePrescriptionRequirement, PrescriptionDto>
    {
        private readonly IPrescriptionService _prescriptionService;

        public CanDispensePrescriptionHandler(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanDispensePrescriptionRequirement requirement,
            PrescriptionDto prescription)
        {
            var role = context.User.FindFirst(ClaimTypes.Role)!.Value;

            //var prescription = await _prescriptionService.GetByIdAsync(prescriptionId);
            if (prescription == null) return;

            if (role == "Admin" || role == "Pharmacist")
            {
                if (!prescription.IsDispensed) // Business Rule 🔥
                    context.Succeed(requirement);
            }
        }
    }
}
