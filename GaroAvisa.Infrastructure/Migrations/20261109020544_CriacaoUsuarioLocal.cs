using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GaroAvisa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoUsuarioLocal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_GS_USUARIO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    SENHA_HASH = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    TIPO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GS_USUARIO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "T_GS_LOCAL",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    APELIDO = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    PAIS = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    FUSO = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false),
                    PRINCIPAL = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RECEBER_ALERTAS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DT_CRIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GS_LOCAL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_T_GS_LOCAL_T_GS_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "T_GS_USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_GS_LOCAL_ID_USUARIO",
                table: "T_GS_LOCAL",
                column: "ID_USUARIO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_GS_LOCAL");

            migrationBuilder.DropTable(
                name: "T_GS_USUARIO");
        }
    }
}
