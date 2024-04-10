using Inventario.Data;
using Inventario.Models;

namespace Inventario.Data
{
    public class AuditoriaService
    {
        private readonly DataContext _context;
        public AuditoriaService(DataContext context)
        {
            _context = context;
        }

        public void RegistrarAuditoria(string tableName, string user, string action, string description)
        {
            var historialEntry = new Auditoria
            {
                Tabla = tableName,
                Usuario = user,
                Acción = action,
                Descripción = description,
                Fecha = DateTime.Now
            };

            _context.historial.Add(historialEntry);
            _context.SaveChanges();
        }
    }
}