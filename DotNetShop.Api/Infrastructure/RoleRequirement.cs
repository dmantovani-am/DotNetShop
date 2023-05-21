using DotNetShop.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace DotNetShop.Api.Infrastructure;

public record RoleRequirement(string Role) : IAuthorizationRequirement
{ }

// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-7.0
public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        if (context.User.HasClaim(nameof(Role), requirement.Role))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
