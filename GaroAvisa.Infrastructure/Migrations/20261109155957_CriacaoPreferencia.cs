using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaroAvisa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoPreferencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_GS_PREFERENCIA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RESUMO_DIARIO_ATIVO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RESUMO_DIARIO_HORARIO = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: false),
                    ALERTAS_DO_DIA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PREVISAO_MUDOU = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    COMPROMISSOS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    SILENCIO_NOTURNO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GS_PREFERENCIA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_GS_PREFERENCIA_T_GS_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "T_GS_USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_GS_PREFERENCIA_ID_USUARIO",
                table: "T_GS_PREFERENCIA",
                column: "ID_USUARIO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_GS_PREFERENCIA");
        }
    }
}
