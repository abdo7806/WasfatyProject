using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.PrescriptionAuthorization.Requirements;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.API.Authorization.PrescriptionAuthorization.Handlers
{
    public class CanEditPrescriptionHandler
        : AuthorizationHandler<CanEditPrescriptionRequirement, PrescriptionDto>
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IDoctorService _doctorService;

        public CanEditPrescriptionHandler(
            IPrescriptionService prescriptionService,
            IDoctorService doctorService)
        {
            _prescriptionService = prescriptionService;
            _doctorService = doctorService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanEditPrescriptionRequirement requirement,
            PrescriptionDto prescription)
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = context.User.FindFirst(ClaimTypes.Role)!.Value;

            //var prescription = await _prescriptionService.GetByIdAsync(prescriptionId);
            if (prescription == null) return;

            if (role == "Admin")
            {
                context.Succeed(requirement);
                return;
            }

            if (role == "Doctor")
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                if (doctor != null && prescription.DoctorId == doctor.Id)
                    context.Succeed(requirement);
            }
        }
    }
}
