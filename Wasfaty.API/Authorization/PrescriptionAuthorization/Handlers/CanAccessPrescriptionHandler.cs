using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.PrescriptionAuthorization.Requirements;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.API.Authorization.PrescriptionAuthorization.Handlers
{
    public class CanAccessPrescriptionHandler
        : AuthorizationHandler<CanAccessPrescriptionRequirement, PrescriptionDto>
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public CanAccessPrescriptionHandler(
            IPrescriptionService prescriptionService,
            IDoctorService doctorService,
            IPatientService patientService)
        {
            _prescriptionService = prescriptionService;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanAccessPrescriptionRequirement requirement,
            PrescriptionDto prescription)
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = context.User.FindFirst(ClaimTypes.Role)!.Value;

            //var prescription = await _prescriptionService.GetByIdAsync(prescriptionId);

            if (prescription == null)
                return;

            // Admin
            if (role == "Admin")
            {
                context.Succeed(requirement);
                return;
            }

            // Pharmacist
            if (role == "Pharmacist")
            {
                context.Succeed(requirement);
                return;
            }

            // Doctor
            if (role == "Doctor")
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);

                if (doctor != null && prescription.DoctorId == doctor.Id)
                    context.Succeed(requirement);
            }

            // Patient
            if (role == "Patient")
            {
                var patient = await _patientService.GetPatientByUserIdAsync(userId);

                if (prescription.PatientId == patient.Id)
                    context.Succeed(requirement);
            }
        }
    }
}
