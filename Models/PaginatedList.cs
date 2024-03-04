namespace Inventario.Models
{
    public class PaginatedList<T>
    {
        public required List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        // Opcional: puedes agregar propiedades adicionales según tus necesidades, como la URL de la página anterior y siguiente.
    }
}