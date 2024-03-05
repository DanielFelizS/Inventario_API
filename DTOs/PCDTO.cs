using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.DTOs
{
    public class PCDTO
    {
        public int Id {get; set;}
        public int Equipo_Id {get; set;}
        public string RAM {get; set;} = "No Tiene";
        public string Disco_duro {get; set;} = "No Tiene";
        public string Procesador {get; set;} = "No Tiene";
        public string Ventilador {get; set;} = "No Tiene";
        public string FuentePoder {get; set;} = "No Tiene";
        public string MotherBoard {get; set;} = "No Tiene";
        public string? Tipo_MotherBoard {get; set;} = "No Tiene";
    }
}