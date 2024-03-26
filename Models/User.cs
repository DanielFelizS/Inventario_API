using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "El primer nombre del usuario es obligatorio")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "El primer apellido del usuario es obligatorio")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "El nombre del usuario es obligatorio")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El email del usuario es obligatorio")]
        [MaxLength(34, ErrorMessage = "El email del usuario no puede tener más de 34 caracteres")]
        public string Email { get;  set; }
        [Required(ErrorMessage = "La contraseña del usuario es obligatoria")]
        public string Password { get; set; }
        // [Required(ErrorMessage = "El rol del usuario es obligatorio")]
        // [MaxLength(16, ErrorMessage = "El rol del usuario no puede tener más de 16 caracteres")]
        // public string UserRol { get; set; }
    }
}