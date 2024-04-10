using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.DTOs
{
    public class PCDTO
    {
        public int Id { get; set; }
        [JsonIgnore]
        public int Equipo_Id {get; set;}
        [NotMapped]
        public string Serial_no { get; set; }
        public string RAM { get; set; } = "No Tiene";
        public string Disco_duro { get; set; } = "No Tiene";
        public string Procesador { get; set; } = "No Tiene";
        public string Ventilador { get; set; } = "No Tiene";
        public string FuentePoder { get; set; } = "No Tiene";
        public string MotherBoard { get; set; } = "No Tiene";
        public string? Tipo_MotherBoard { get; set; } = "No Tiene";
    }
}