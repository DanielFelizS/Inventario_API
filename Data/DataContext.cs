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
        public DbSet<Departamento> departamento { get; set; }
        public DbSet<PC> Computer {get; set;}
        public DbSet<User> usuarios { get; set; }
        public DbSet<Auditoria> historial { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Dispositivo>()
                .HasOne(d => d.departamento)
                .WithMany(dpto => dpto.Dispositivos)
                .HasForeignKey(d => d.DepartamentoId)
                .HasConstraintName("FK_Departamento");

            modelBuilder.Entity<PC>()
                .HasOne(d => d.Dispositivos)
                .WithMany(dpto => dpto.Computer)
                .HasForeignKey(d => d.Equipo_Id)
                .HasConstraintName("FK_Dispositivo");

            modelBuilder.Entity<Dispositivo>()
                .HasIndex(d => d.Serial_no)
                .IsUnique()
                .HasFilter("[Serial_no] IS NOT NULL AND [Serial_no]<> 'No Tiene'");

            modelBuilder.Entity<Dispositivo>()
                .HasIndex(d => d.Cod_inventario)
                .IsUnique()
                .HasFilter("[Cod_inventario] IS NOT NULL AND [Cod_inventario]<> 'No Tiene'");

            modelBuilder.Entity<Dispositivo>()
                .HasIndex(d => d.Bienes_nacionales)
                .IsUnique()
                .HasFilter("[Bienes_nacionales] IS NOT NULL AND [Bienes_nacionales] <> 0");

            modelBuilder.Entity<Departamento>()
                .HasIndex(d => d.Nombre)
                .IsUnique();

            modelBuilder.Entity<Departamento>()
                .HasIndex(d => d.Encargado)
                .IsUnique();

            modelBuilder.Entity<PC>()
                .HasIndex(d => d.Equipo_Id)
                .IsUnique();   
            
            modelBuilder.Entity<PC>()
                .HasIndex(d => d.RAM)
                .HasFilter("[RAM] IS NOT NULL AND [RAM]<> 'No Tiene'");

            modelBuilder.Entity<PC>()
                .HasIndex(d => d.Disco_duro)
                .HasFilter("[Disco_duro] IS NOT NULL AND [Disco_duro]<> 'No Tiene'");

            modelBuilder.Entity<PC>()
                .HasIndex(d => d.Procesador)
                .HasFilter("[Procesador] IS NOT NULL AND [Procesador]<> 'No Tiene'");

            modelBuilder.Entity<PC>()
                .HasIndex(d => d.Ventilador)
                .HasFilter("[Ventilador] IS NOT NULL AND [Ventilador]<> 'No Tiene'");

            modelBuilder.Entity<PC>()
                .HasIndex(d => d.FuentePoder)
                .HasFilter("[FuentePoder] IS NOT NULL AND [FuentePoder]<> 'No Tiene'");

            modelBuilder.Entity<PC>()
                .HasIndex(d => d.MotherBoard)
                .HasFilter("[MotherBoard] IS NOT NULL AND [MotherBoard]<> 'No Tiene'");
            
            modelBuilder.Entity<User>()
                .HasIndex(d => d.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(d => d.UserName)
                .IsUnique();

            modelBuilder.Entity<Auditoria>()
                .ToTable("historial");
            
            modelBuilder.Entity<Dispositivo>()
                .ToTable("Dispositivos", options =>
                {
                    options.IsTemporal();
                });
    }
}
}