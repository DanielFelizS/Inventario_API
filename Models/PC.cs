using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.Models
{
    public class PC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "El Id del equipo no debe ser nulo")]
        public int Equipo_Id { get; set; }
        // [Required(ErrorMessage = "El nombre del equipo no debe ser nulo")]
        // public string Nombre_equipo { get; set; }
        public Dispositivo Dispositivos { get; set; }
        [Required(ErrorMessage = "La memoria RAM no debe ser nulo")]
        public string RAM { get; set; } = "No Tiene";
        [Required(ErrorMessage = "El Disco duro del PC no debe ser nulo")]
        public string Disco_duro { get; set; } = "No Tiene";
        [Required(ErrorMessage = "El Procesador del PC no debe ser nulo")]
        public string Procesador { get; set; } = "No Tiene";
        [Required(ErrorMessage = "El Ventilador del PC no debe ser nulo")]
        public string Ventilador { get; set; } = "No Tiene";
        [Required(ErrorMessage = "La fuente de poder del PC no debe ser nulo")]
        public string FuentePoder { get; set; } = "No Tiene";
        [Required(ErrorMessage = "La Tarjeta Madre del PC no debe ser nulo")]
        public string MotherBoard { get; set; } = "No Tiene";
        public string? Tipo_MotherBoard { get; set; } = "No Tiene";

    }
}