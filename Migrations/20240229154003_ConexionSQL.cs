using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario.Migrations
{
    /// <inheritdoc />
    public partial class ConexionSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descripción = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Encargado = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRol = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_equipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Serial_no = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Cod_inventario = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Bienes_nacionales = table.Column<int>(type: "int", nullable: false),
                    Fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Propietario_equipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispositivos_departamento_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "departamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Computer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Equipo_Id = table.Column<int>(type: "int", nullable: false),
                    RAM = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Disco_duro = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Procesador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ventilador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FuentePoder = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MotherBoard = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tipo_MotherBoard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispositivoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Computer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Computer_Dispositivos_DispositivoId",
                        column: x => x.DispositivoId,
                        principalTable: "Dispositivos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Computer_Disco_duro",
                table: "Computer",
                column: "Disco_duro",
                filter: "[Disco_duro] IS NOT NULL AND [Disco_duro]<> 'No Tiene'");

            migrationBuilder.CreateIndex(
                name: "IX_Computer_DispositivoId",
                table: "Computer",
                column: "DispositivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Computer_Equipo_Id",
                table: "Computer",
                column: "Equipo_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Computer_FuentePoder",
                table: "Computer",
                column: "FuentePoder",
                filter: "[FuentePoder] IS NOT NULL AND [FuentePoder]<> 'No Tiene'");

            migrationBuilder.CreateIndex(
                name: "IX_Computer_MotherBoard",
                table: "Computer",
                column: "MotherBoard",
                filter: "[MotherBoard] IS NOT NULL AND [MotherBoard]<> 'No Tiene'");

            migrationBuilder.CreateIndex(
                name: "IX_Computer_Procesador",
                table: "Computer",
                column: "Procesador",
                filter: "[Procesador] IS NOT NULL AND [Procesador]<> 'No Tiene'");

            migrationBuilder.CreateIndex(
                name: "IX_Computer_RAM",
                table: "Computer",
                column: "RAM",
                filter: "[RAM] IS NOT NULL AND [RAM]<> 'No Tiene'");

            migrationBuilder.CreateIndex(
                name: "IX_Computer_Ventilador",
                table: "Computer",
                column: "Ventilador",
                filter: "[Ventilador] IS NOT NULL AND [Ventilador]<> 'No Tiene'");

            migrationBuilder.CreateIndex(
                name: "IX_departamento_Encargado",
                table: "departamento",
                column: "Encargado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_departamento_Nombre",
                table: "departamento",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_Bienes_nacionales",
                table: "Dispositivos",
                column: "Bienes_nacionales",
                unique: true,
                filter: "[Bienes_nacionales] IS NOT NULL AND [Bienes_nacionales] <> 0");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_Cod_inventario",
                table: "Dispositivos",
                column: "Cod_inventario",
                unique: true,
                filter: "[Cod_inventario] IS NOT NULL AND [Cod_inventario]<> 'No Tiene'");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_DepartamentoId",
                table: "Dispositivos",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_Serial_no",
                table: "Dispositivos",
                column: "Serial_no",
                unique: true,
                filter: "[Serial_no] IS NOT NULL AND [Serial_no]<> 'No Tiene'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Computer");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "Dispositivos");

            migrationBuilder.DropTable(
                name: "departamento");
        }
    }
}
