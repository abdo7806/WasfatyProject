using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wasfaty.API.Authorization.DispenseRecordAuthorization.Requirements;
using Wasfaty.Application.Constants;
using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.API.Authorization.DispenseRecordAuthorization.Handlers
{
    public class CanAccessDispenseRecordHandler :
    AuthorizationHandler<CanAccessDispenseRecordRequirement, DispenseRecordDto>,
    IAuthorizationHandler
    {
        private readonly IPharmacistService _pharmacistService;

        public CanAccessDispenseRecordHandler(IPharmacistService pharmacistService)
        {
            _pharmacistService = pharmacistService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanAccessDispenseRecordRequirement requirement,
            DispenseRecordDto resource)
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            // Admin يقدر يشوف كل شيء
            if (role == Roles.Admin)
            {
                context.Succeed(requirement);
                return;
            }

            // الصيدلي فقط يشوف سجلات صيدليته
            if (role == Roles.Pharmacist)
            {
                var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(userId);

                if (pharmacist != null && pharmacist.PharmacyId == resource.PharmacyId)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
