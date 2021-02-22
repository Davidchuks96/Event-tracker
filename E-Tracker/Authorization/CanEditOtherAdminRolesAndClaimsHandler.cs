using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Tracker.Authorization
{
    public class CanEditOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if(authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            string loggedInId =
                context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string adminBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            if(context.User.IsInRole("SuperAdmin") &&
                context.User.HasClaim(claim => claim.Type == CustomClaims.Permission && claim.Value == CustomClaimsValues.EditRole) && 
                adminBeingEdited.ToLower() != loggedInId.ToLower())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
