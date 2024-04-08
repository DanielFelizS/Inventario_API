using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.Models
{
    public class Departamento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del departamento es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Una descripción es obligatorio")]
        public string Descripción { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
        [DataType(DataType.Date)]
        public DateTime? Fecha_creacion { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "El nombre del encargado es obligatorio")]
        public string Encargado { get; set; }
        [Required(ErrorMessage = "Los dispositivos que tiene el departamento es obligatorio")]
        public virtual ICollection<Dispositivo> Dispositivos { get; set; }
    }
}