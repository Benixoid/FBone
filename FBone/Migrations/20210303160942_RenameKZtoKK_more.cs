using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class RenameKZtoKK_more : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name_KZ",
                table: "tArea",
                newName: "Name_KK");

            migrationBuilder.RenameColumn(
                name: "Name_KZ",
                table: "tActProtect",
                newName: "Name_KK");

            migrationBuilder.RenameColumn(
                name: "Name_KZ",
                table: "tActImpact",
                newName: "Name_KK");

            migrationBuilder.RenameColumn(
                name: "Name_KZ",
                table: "tActCause",
                newName: "Name_KK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name_KK",
                table: "tArea",
                newName: "Name_KZ");

            migrationBuilder.RenameColumn(
                name: "Name_KK",
                table: "tActProtect",
                newName: "Name_KZ");

            migrationBuilder.RenameColumn(
                name: "Name_KK",
                table: "tActImpact",
                newName: "Name_KZ");

            migrationBuilder.RenameColumn(
                name: "Name_KK",
                table: "tActCause",
                newName: "Name_KZ");
        }
    }
}
