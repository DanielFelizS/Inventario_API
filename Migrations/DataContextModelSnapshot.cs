﻿// <auto-generated />
using System;
using Inventario.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Inventario.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Inventario.Models.Auditoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Acción")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descripción")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<string>("Tabla")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Usuario")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("historial", (string)null);
                });

            modelBuilder.Entity("Inventario.Models.Departamento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripción")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Encargado")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("Fecha_creacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Encargado")
                        .IsUnique();

                    b.HasIndex("Nombre")
                        .IsUnique();

                    b.ToTable("departamento");
                });

            modelBuilder.Entity("Inventario.Models.Dispositivo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Bienes_nacionales")
                        .HasColumnType("int");

                    b.Property<string>("Cod_inventario")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DepartamentoId")
                        .HasColumnType("int");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<DateTime?>("Fecha_modificacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Marca")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Modelo")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Nombre_equipo")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Nombre_windows")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Propietario_equipo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Serial_no")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("Bienes_nacionales")
                        .IsUnique()
                        .HasFilter("[Bienes_nacionales] IS NOT NULL AND [Bienes_nacionales] <> 0");

                    b.HasIndex("Cod_inventario")
                        .IsUnique()
                        .HasFilter("[Cod_inventario] IS NOT NULL AND [Cod_inventario]<> 'No Tiene'");

                    b.HasIndex("DepartamentoId");

                    b.HasIndex("Serial_no")
                        .IsUnique()
                        .HasFilter("[Serial_no] IS NOT NULL AND [Serial_no]<> 'No Tiene'");

                    b.ToTable("Dispositivos");
                });

            modelBuilder.Entity("Inventario.Models.PC", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Disco_duro")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Equipo_Id")
                        .HasColumnType("int");

                    b.Property<string>("FuentePoder")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MotherBoard")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Procesador")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RAM")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Tipo_MotherBoard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ventilador")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Disco_duro")
                        .HasFilter("[Disco_duro] IS NOT NULL AND [Disco_duro]<> 'No Tiene'");

                    b.HasIndex("Equipo_Id")
                        .IsUnique();

                    b.HasIndex("FuentePoder")
                        .HasFilter("[FuentePoder] IS NOT NULL AND [FuentePoder]<> 'No Tiene'");

                    b.HasIndex("MotherBoard")
                        .HasFilter("[MotherBoard] IS NOT NULL AND [MotherBoard]<> 'No Tiene'");

                    b.HasIndex("Procesador")
                        .HasFilter("[Procesador] IS NOT NULL AND [Procesador]<> 'No Tiene'");

                    b.HasIndex("RAM")
                        .HasFilter("[RAM] IS NOT NULL AND [RAM]<> 'No Tiene'");

                    b.HasIndex("Ventilador")
                        .HasFilter("[Ventilador] IS NOT NULL AND [Ventilador]<> 'No Tiene'");

                    b.ToTable("Computer");
                });

            modelBuilder.Entity("Inventario.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("nvarchar(34)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("usuarios");
                });

            modelBuilder.Entity("Inventario.Models.Dispositivo", b =>
                {
                    b.HasOne("Inventario.Models.Departamento", "departamento")
                        .WithMany("Dispositivos")
                        .HasForeignKey("DepartamentoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Departamento");

                    b.Navigation("departamento");
                });

            modelBuilder.Entity("Inventario.Models.PC", b =>
                {
                    b.HasOne("Inventario.Models.Dispositivo", "Dispositivos")
                        .WithMany("Computer")
                        .HasForeignKey("Equipo_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Dispositivo");

                    b.Navigation("Dispositivos");
                });

            modelBuilder.Entity("Inventario.Models.Departamento", b =>
                {
                    b.Navigation("Dispositivos");
                });

            modelBuilder.Entity("Inventario.Models.Dispositivo", b =>
                {
                    b.Navigation("Computer");
                });
#pragma warning restore 612, 618
        }
    }
}
