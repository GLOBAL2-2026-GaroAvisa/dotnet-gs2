using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaroAvisa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenomeiaApelidoParaEtiqueta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "APELIDO",
                table: "T_GS_LOCAL",
                newName: "ETIQUETA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ETIQUETA",
                table: "T_GS_LOCAL",
                newName: "APELIDO");
        }
    }
}
