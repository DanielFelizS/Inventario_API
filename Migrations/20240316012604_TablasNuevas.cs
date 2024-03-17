using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario.Migrations
{
    /// <inheritdoc />
    public partial class TablasNuevas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Computer_Dispositivos_DispositivoId",
                table: "Computer");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispositivos_departamento_DepartamentoId",
                table: "Dispositivos");

            migrationBuilder.DropIndex(
                name: "IX_Computer_DispositivoId",
                table: "Computer");

            migrationBuilder.DropColumn(
                name: "DispositivoId",
                table: "Computer");

            migrationBuilder.AlterColumn<string>(
                name: "UserRol",
                table: "usuarios",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "usuarios",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositivo",
                table: "Computer",
                column: "Equipo_Id",
                principalTable: "Dispositivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departamento",
                table: "Dispositivos",
                column: "DepartamentoId",
                principalTable: "departamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositivo",
                table: "Computer");

            migrationBuilder.DropForeignKey(
                name: "FK_Departamento",
                table: "Dispositivos");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_Email",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "UserRol",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DispositivoId",
                table: "Computer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Computer_DispositivoId",
                table: "Computer",
                column: "DispositivoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Computer_Dispositivos_DispositivoId",
                table: "Computer",
                column: "DispositivoId",
                principalTable: "Dispositivos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositivos_departamento_DepartamentoId",
                table: "Dispositivos",
                column: "DepartamentoId",
                principalTable: "departamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
