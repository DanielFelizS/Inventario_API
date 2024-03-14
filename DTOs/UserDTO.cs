using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get;  set; }
        public string Password { get; set; }
        public string UserRol { get; set; }
    }
}