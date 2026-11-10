using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaroAvisa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoRegraAlerta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_GS_REGRA_ALERTA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TIPO = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    LIMITE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    GRAVIDADE = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    TITULO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    ATIVA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GS_REGRA_ALERTA", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_GS_REGRA_ALERTA");
        }
    }
}
