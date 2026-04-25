using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EsperancaSolidaria.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TabelaDoacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampanhaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataDoacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    ReferenciaPagamento = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doacao_Campanha_CampanhaId",
                        column: x => x.CampanhaId,
                        principalTable: "Campanha",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Doacao_Usuario_DoadorId",
                        column: x => x.DoadorId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doacao_CampanhaId",
                table: "Doacao",
                column: "CampanhaId");

            migrationBuilder.CreateIndex(
                name: "IX_Doacao_DoadorId",
                table: "Doacao",
                column: "DoadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Doacao_ReferenciaPagamento",
                table: "Doacao",
                column: "ReferenciaPagamento",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doacao");
        }
    }
}
