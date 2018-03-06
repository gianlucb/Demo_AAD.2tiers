using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AAD._2tiers.Api
{
    public class HasScopeRequirement : AuthorizationHandler<HasScopeRequirement>, IAuthorizationRequirement
    {
        private readonly string issuer;
        private readonly string scope;

        public HasScopeRequirement(string scope)
        {
            this.scope = scope;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "http://schemas.microsoft.com/identity/claims/scope"))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var scopes = context.User.FindFirst(c => c.Type == "http://schemas.microsoft.com/identity/claims/scope").Value.Split(' ');

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == scope))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
