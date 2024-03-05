using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Inventario.Data;
using Inventario.Models;

namespace Inventario.Models
{
    public class Departamento
    {
        // public Departamento()
        // {
        //     Dispositivos = new HashSet<Dispositivo>();
        // }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del departamento es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Una descripción es obligatorio")]
        public string Descripción { get; set; }
        public DateTime? Fecha_creacion { get; set; }
        [Required(ErrorMessage = "El nombre del encargado es obligatorio")]
        public string Encargado { get; set; }
        [Required(ErrorMessage = "Los dispositivos que tiene el departamento es obligatorio")]
        public virtual ICollection<Dispositivo> Dispositivos { get; set; }
    }
}