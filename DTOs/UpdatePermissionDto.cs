using System.ComponentModel.DataAnnotations;

namespace Inventario.DTOs
{
    public class UpdatePermissionDto
    {
        [Required(ErrorMessage = "El nombre del usuario es obligatorio")]
        public string UserName { get; set; }
    }
}