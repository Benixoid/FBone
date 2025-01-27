using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedNewFieldsToArea1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TagAlarmDisabled",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagAlarmDisabledYestd",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagAlarmInhibited",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagAlarmInhibitedYestd",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagBypasActive",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagBypasDaily",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagForcesDaily",
                table: "tArea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagAlarmDisabled",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "TagAlarmDisabledYestd",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "TagAlarmInhibited",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "TagAlarmInhibitedYestd",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "TagBypasActive",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "TagBypasDaily",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "TagForcesDaily",
                table: "tArea");
        }
    }
}
