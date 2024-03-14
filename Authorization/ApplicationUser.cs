using Microsoft.AspNetCore.Identity;

namespace Inventario.Authorization
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}