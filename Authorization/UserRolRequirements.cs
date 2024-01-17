using Microsoft.AspNetCore.Authorization;

namespace Inventario.Authorization
{
    public class UserRolRequirements : IAuthorizationRequirement
    {
        public string UserRol { get; }

        public UserRolRequirements(string userRol)
        {
            UserRol = userRol;
        }
    }
}
