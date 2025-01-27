using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedIsTranslatedFieldsToActTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCauseTranslated",
                table: "tAct",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsImpactTranslated",
                table: "tAct",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProtectTranslated",
                table: "tAct",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCauseTranslated",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "IsImpactTranslated",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "IsProtectTranslated",
                table: "tAct");
        }
    }
}
