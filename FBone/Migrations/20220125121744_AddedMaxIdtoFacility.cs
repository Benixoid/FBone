using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedMaxIdtoFacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Alarm_maxId",
                table: "tFacility",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Force_maxId",
                table: "tFacility",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alarm_maxId",
                table: "tFacility");

            migrationBuilder.DropColumn(
                name: "Force_maxId",
                table: "tFacility");
        }
    }
}
