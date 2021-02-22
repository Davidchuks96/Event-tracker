using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace E_Tracker.Authorization
{
    public class DeleteAuthorizationHandler : AuthorizationHandler<AccountAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountAuthorizationRequirement requirement)
        {
            if (context.User == null) return Task.CompletedTask;
            if (context.User.HasClaim(CustomClaims.Permission, CustomClaimsValues.Delete)) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
