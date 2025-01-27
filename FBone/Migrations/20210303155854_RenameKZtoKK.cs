using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class RenameKZtoKK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name_KZ",
                table: "tUser",
                newName: "Name_Kk");

            migrationBuilder.RenameColumn(
                name: "ProtectKz",
                table: "tAct",
                newName: "ProtectKk");

            migrationBuilder.RenameColumn(
                name: "ImpactKz",
                table: "tAct",
                newName: "ImpactKk");

            migrationBuilder.RenameColumn(
                name: "CauseKz",
                table: "tAct",
                newName: "CauseKk");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name_Kk",
                table: "tUser",
                newName: "Name_KZ");

            migrationBuilder.RenameColumn(
                name: "ProtectKk",
                table: "tAct",
                newName: "ProtectKz");

            migrationBuilder.RenameColumn(
                name: "ImpactKk",
                table: "tAct",
                newName: "ImpactKz");

            migrationBuilder.RenameColumn(
                name: "CauseKk",
                table: "tAct",
                newName: "CauseKz");
        }
    }
}
