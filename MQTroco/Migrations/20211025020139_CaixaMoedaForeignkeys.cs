using Microsoft.EntityFrameworkCore.Migrations;

namespace MQTroco.Migrations
{
    public partial class CaixaMoedaForeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CaixaMoedas_CaixaModelId",
                table: "CaixaMoedas",
                column: "CaixaModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaMoedas_MoedaModelId",
                table: "CaixaMoedas",
                column: "MoedaModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaixaMoedas_Caixas_CaixaModelId",
                table: "CaixaMoedas",
                column: "CaixaModelId",
                principalTable: "Caixas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CaixaMoedas_Moedas_MoedaModelId",
                table: "CaixaMoedas",
                column: "MoedaModelId",
                principalTable: "Moedas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaixaMoedas_Caixas_CaixaModelId",
                table: "CaixaMoedas");

            migrationBuilder.DropForeignKey(
                name: "FK_CaixaMoedas_Moedas_MoedaModelId",
                table: "CaixaMoedas");

            migrationBuilder.DropIndex(
                name: "IX_CaixaMoedas_CaixaModelId",
                table: "CaixaMoedas");

            migrationBuilder.DropIndex(
                name: "IX_CaixaMoedas_MoedaModelId",
                table: "CaixaMoedas");
        }
    }
}
