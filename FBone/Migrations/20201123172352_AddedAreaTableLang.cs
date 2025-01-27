using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedAreaTableLang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "tArea",
                newName: "Name_RU");

            migrationBuilder.AddColumn<string>(
                name: "Name_EN",
                table: "tArea",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name_KZ",
                table: "tArea",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name_EN",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Name_KZ",
                table: "tArea");

            migrationBuilder.RenameColumn(
                name: "Name_RU",
                table: "tArea",
                newName: "Name");
        }
    }
}
