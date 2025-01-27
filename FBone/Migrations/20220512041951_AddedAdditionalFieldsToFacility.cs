using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedAdditionalFieldsToFacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TranslatorEmail",
                table: "tFacility",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isWriteToPIRequired",
                table: "tFacility",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslatorEmail",
                table: "tFacility");

            migrationBuilder.DropColumn(
                name: "isWriteToPIRequired",
                table: "tFacility");
        }
    }
}
