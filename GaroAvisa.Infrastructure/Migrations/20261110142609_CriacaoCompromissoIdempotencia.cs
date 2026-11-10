using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaroAvisa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoCompromissoIdempotencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_GS_COMPROMISSO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_LOCAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    DT_COMPROMISSO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GS_COMPROMISSO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_GS_COMPROMISSO_T_GS_LOCAL_ID_LOCAL",
                        column: x => x.ID_LOCAL,
                        principalTable: "T_GS_LOCAL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_GS_IDEMPOTENCIA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CHAVE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ROTA = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    HASH_REQUISICAO = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false),
                    STATUS_CODE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RESPOSTA_JSON = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GS_IDEMPOTENCIA", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_GS_COMPROMISSO_ID_LOCAL",
                table: "T_GS_COMPROMISSO",
                column: "ID_LOCAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_GS_COMPROMISSO");

            migrationBuilder.DropTable(
                name: "T_GS_IDEMPOTENCIA");
        }
    }
}
