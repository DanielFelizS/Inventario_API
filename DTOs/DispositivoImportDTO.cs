using System;

namespace Inventario.DTOs
{
    public class DispositivoImportDTO
    {
        public string Nombre_equipo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Estado { get; set; }
        public string Serial_no { get; set; }
        public string Cod_inventario { get; set; }
        public int Bienes_nacionales { get; set; }
        public DateTime Fecha_modificacion { get; set; }
        public string Propietario_equipo { get; set; }
        public string Nombre_departamento { get; set; }
    }
}
