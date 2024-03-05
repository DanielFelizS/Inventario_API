using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.DTOs
{
    public class DepartamentoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripción { get; set; }
        public DateTime? Fecha_creacion { get; set; }
        public string Encargado { get; set; }
    }
}