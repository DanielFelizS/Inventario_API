using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventario.Data;

namespace Inventario.Models
{
    public partial class Dispositivo
    {
        // public Dispositivo()
        // {
        //     Computer = new HashSet<PC>();
        // }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
        [MaxLength(20, ErrorMessage = "El nombre no puede tener más de 20 caracteres")]
        public string Nombre_equipo { get; set; }

        [Required(ErrorMessage = "La marca del equipo es obligatorio")]
        [MaxLength(20, ErrorMessage = "La marca del equipo no puede tener más de 20 caracteres")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "El modelo del equipo es obligatorio")]
        [MaxLength(20, ErrorMessage = "El modelo del equipo no puede tener más de 20 caracteres")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "El estado del equipo es obligatorio")]
        [MaxLength(15, ErrorMessage = "El estado del equipo no puede tener más de 15 caracteres")]
        public string Estado { get; set; }

        [MaxLength(20, ErrorMessage = "El número de serie del equipo no puede tener más de 20 caracteres")]
        // [CustomValidation(typeof(Dispositivo), "ValidateSerialUnico")]
        public string? Serial_no { get; set; } = "No Tiene";

        // [CustomValidation(typeof(Dispositivo), "ValidateInviUnico")]
        public string? Cod_inventario { get; set; } = "No Tiene";

        [Required(ErrorMessage = "Bienes Nacionales es obligatorio")]
        // [CustomValidation(typeof(Dispositivo), "ValidateBienesNacionalesUnico")]
        public int Bienes_nacionales { get; set; } = 0;
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode=true)]
        [DataType(DataType.Date)]
        public DateTime? Fecha_modificacion { get; set; } = DateTime.Now;
        public string Propietario_equipo { get; set; }

        // Relación entre el nombre del departamento y la clase Dispositivos
        public int DepartamentoId{ get; set; }

        public Departamento departamento {get; set;}
        // public PC Computer {get; set;}

        public virtual ICollection<PC> Computer { get; set; }
    }
}