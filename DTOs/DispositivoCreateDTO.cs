namespace Inventario.DTOs
{
    public class DispositivoCreateDTO
    {
        public int Id { get; set; }
        public string Nombre_equipo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Estado { get; set; }
        public string Serial_no { get; set; } = "No Tiene";
        public string Cod_inventario { get; set; } = "No Tiene";
        public int Bienes_nacionales { get; set; } = 0;
        public DateTime? Fecha_modificacion { get; set; }
        public string Propietario_equipo { get; set; }
        public int DepartamentoId { get; set; }
    }
}
