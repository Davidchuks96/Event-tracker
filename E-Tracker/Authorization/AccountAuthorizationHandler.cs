using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Authorization
{
    public class AccountAuthorizationHandler : AuthorizationHandler<AccountAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountAuthorizationRequirement requirement)
        {
            if (context.User == null) return Task.CompletedTask;
            if (context.User.HasClaim(CustomClaims.Permission, requirement.Operation)) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
