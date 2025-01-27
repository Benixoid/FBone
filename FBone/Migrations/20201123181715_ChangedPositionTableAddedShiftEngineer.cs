using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class ChangedPositionTableAddedShiftEngineer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShiftEngineer",
                table: "tPosition",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShiftEngineer",
                table: "tPosition");
        }
    }
}
