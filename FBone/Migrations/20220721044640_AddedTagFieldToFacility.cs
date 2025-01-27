using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedTagFieldToFacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TagBypassTotal",
                table: "tFacility",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagForcesTotal",
                table: "tFacility",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagBypassTotal",
                table: "tFacility");

            migrationBuilder.DropColumn(
                name: "TagForcesTotal",
                table: "tFacility");
        }
    }
}
