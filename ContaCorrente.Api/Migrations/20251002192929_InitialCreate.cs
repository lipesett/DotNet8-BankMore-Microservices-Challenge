using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContaCorrente.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChavesIdempotencia",
                columns: table => new
                {
                    IdChaveIdempotencia = table.Column<string>(type: "TEXT", nullable: false),
                    Requisicao = table.Column<string>(type: "TEXT", nullable: true),
                    Resultado = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChavesIdempotencia", x => x.IdChaveIdempotencia);
                });

            migrationBuilder.CreateTable(
                name: "ContasCorrentes",
                columns: table => new
                {
                    IdContaCorrente = table.Column<string>(type: "TEXT", nullable: false),
                    Numero = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    Senha = table.Column<string>(type: "TEXT", nullable: true),
                    Salt = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasCorrentes", x => x.IdContaCorrente);
                });

            migrationBuilder.CreateTable(
                name: "Movimentos",
                columns: table => new
                {
                    IdMovimento = table.Column<string>(type: "TEXT", nullable: false),
                    IdContaCorrente = table.Column<string>(type: "TEXT", nullable: false),
                    DataMovimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoMovimento = table.Column<char>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimentos", x => x.IdMovimento);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChavesIdempotencia");

            migrationBuilder.DropTable(
                name: "ContasCorrentes");

            migrationBuilder.DropTable(
                name: "Movimentos");
        }
    }
}
