using Microsoft.EntityFrameworkCore.Migrations;

namespace MQTroco.Migrations
{
    public partial class CaixaMoeda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaixaMoedas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CaixaModelId = table.Column<string>(type: "TEXT", nullable: false),
                    MoedaModelId = table.Column<string>(type: "TEXT", nullable: false),
                    QtdMoeda = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaixaMoedas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaixaMoedas");
        }
    }
}
