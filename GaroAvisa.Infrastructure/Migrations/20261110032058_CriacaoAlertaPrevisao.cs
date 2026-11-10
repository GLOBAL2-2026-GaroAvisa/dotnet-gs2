using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaroAvisa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoAlertaPrevisao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_GS_ALERTA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_LOCAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DT_PREVISAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TIPO = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    TITULO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    GRAVIDADE = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    VALOR_PREVISTO = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    LIDO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GS_ALERTA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_GS_ALERTA_T_GS_LOCAL_ID_LOCAL",
                        column: x => x.ID_LOCAL,
                        principalTable: "T_GS_LOCAL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_GS_PREVISAO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_LOCAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DT_PREVISAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CODIGO_TEMPO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TEMP_MAXIMA = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    TEMP_MINIMA = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    CHUVA_MM = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    PROB_CHUVA = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    RAJADA_VENTO = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GS_PREVISAO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_GS_PREVISAO_T_GS_LOCAL_ID_LOCAL",
                        column: x => x.ID_LOCAL,
                        principalTable: "T_GS_LOCAL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_GS_ALERTA_ID_LOCAL",
                table: "T_GS_ALERTA",
                column: "ID_LOCAL");

            migrationBuilder.CreateIndex(
                name: "IX_T_GS_PREVISAO_ID_LOCAL",
                table: "T_GS_PREVISAO",
                column: "ID_LOCAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_GS_ALERTA");

            migrationBuilder.DropTable(
                name: "T_GS_PREVISAO");
        }
    }
}
