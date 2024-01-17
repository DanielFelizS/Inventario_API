using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Inventario.Authorization
{
    public class UserRolAuthorizationHandler : AuthorizationHandler<UserRolRequirements>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRolRequirements requirement)
        {
            if (context.User.IsInRole(requirement.UserRol))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
