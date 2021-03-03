using Microsoft.EntityFrameworkCore.Migrations;

namespace Casino_Royale_Api.Migrations
{
    public partial class updatemigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(nullable: false),
                    currentMoney = table.Column<double>(nullable: true),
                    totalEarned = table.Column<double>(nullable: true),
                    totalLost = table.Column<double>(nullable: true),
                    active = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
