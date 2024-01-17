using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario.Migrations
{
    /// <inheritdoc />
    public partial class conexionInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_equipo = table.Column<string>(type: "varchar(30)", nullable: true),
                    Marca = table.Column<string>(type: "varchar(30)", nullable: true),
                    Modelo = table.Column<string>(type: "varchar(30)", nullable: true),
                    Estado = table.Column<string>(type: "varchar(30)", nullable: true),
                    Serial_no = table.Column<string>(type: "varchar(30)", nullable: true),
                    Cod_inventario = table.Column<string>(type: "varchar(30)", nullable: true),
                    Bienes_nacionales = table.Column<int>(type: "int", nullable: true),
                    Fecha_modificacion = table.Column<DateTime>(type: "date", nullable: true),
                    Propietario_equipo = table.Column<string>(type: "varchar(30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dispositivos");
        }
    }
}
