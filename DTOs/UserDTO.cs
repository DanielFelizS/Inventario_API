using Microsoft.AspNetCore.Identity;

namespace Inventario.DTOs
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        // public string UserRol { get; set; }
    }
}