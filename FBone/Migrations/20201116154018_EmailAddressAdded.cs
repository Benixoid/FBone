using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class EmailAddressAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "tPosition");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "tUser",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "tUser");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "tPosition",
                nullable: false,
                defaultValue: "");
        }
    }
}
