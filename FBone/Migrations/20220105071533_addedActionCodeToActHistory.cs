using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class addedActionCodeToActHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionCode",
                table: "ActHistory",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionCode",
                table: "ActHistory");
        }
    }
}
