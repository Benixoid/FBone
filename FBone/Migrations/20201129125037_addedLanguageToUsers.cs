using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class addedLanguageToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lang",
                table: "tUser",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lang",
                table: "tUser");
        }
    }
}
