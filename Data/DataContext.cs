using Microsoft.EntityFrameworkCore;
using Inventario.Models;

namespace Inventario.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Dispositivo> Dispositivos { get; set; }
        public DbSet<User> usuarios { get; set; }
    }
}