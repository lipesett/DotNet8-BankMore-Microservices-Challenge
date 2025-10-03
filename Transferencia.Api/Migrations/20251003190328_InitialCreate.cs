using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transferencia.Api.Migrations
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
                name: "Transferencias",
                columns: table => new
                {
                    IdTransferencia = table.Column<string>(type: "TEXT", nullable: false),
                    IdContaCorrente_Origem = table.Column<string>(type: "TEXT", nullable: false),
                    IdContaCorrente_Destino = table.Column<string>(type: "TEXT", nullable: false),
                    DataMovimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencias", x => x.IdTransferencia);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChavesIdempotencia");

            migrationBuilder.DropTable(
                name: "Transferencias");
        }
    }
}
