using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class RemovedTextActionFromActHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "ActHistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "ActHistory",
                nullable: true);
        }
    }
}
