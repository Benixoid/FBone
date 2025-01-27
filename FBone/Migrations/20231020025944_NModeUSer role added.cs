using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class NModeUSerroleadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNModeUser",
                table: "tPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNModeUser",
                table: "tPosition");
        }
    }
}
