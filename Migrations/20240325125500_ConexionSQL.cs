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
                    Nombre = table.Column<string>(type: "varchar(450)", nullable: false),
                    Descripción = table.Column<string>(type: "varchar(100)", nullable: false),
                    Fecha_creacion = table.Column<DateTime>(type: "date", nullable: true),
                    Encargado = table.Column<string>(type: "varchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "historial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tabla = table.Column<string>(type: "varchar(20)", nullable: true),
                    Usuario = table.Column<string>(type: "varchar(max)", nullable: true),
                    Acción = table.Column<string>(type: "varchar(20)", nullable: true),
                    Descripción = table.Column<string>(type: "varchar(100)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Nombre_equipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Marca = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Modelo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Estado = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Serial_no = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Cod_inventario = table.Column<string>(type: "nvarchar(450)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Bienes_nacionales = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Fecha_modificacion = table.Column<DateTime>(type: "date", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    Propietario_equipo = table.Column<string>(type: "varchar(60)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departamento",
                        column: x => x.DepartamentoId,
                        principalTable: "departamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

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
                    Tipo_MotherBoard = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Computer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispositivo",
                        column: x => x.Equipo_Id,
                        principalTable: "Dispositivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Computer_Disco_duro",
                table: "Computer",
                column: "Disco_duro",
                filter: "[Disco_duro] IS NOT NULL AND [Disco_duro]<> 'No Tiene'");

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

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_UserName",
                table: "usuarios",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Computer");

            migrationBuilder.DropTable(
                name: "historial");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "Dispositivos")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DispositivosHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "departamento");
        }
    }
}
