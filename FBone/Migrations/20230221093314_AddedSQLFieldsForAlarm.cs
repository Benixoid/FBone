using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class AddedSQLFieldsForAlarm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxId",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxIdAlarm",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SQLqueryAlarm",
                table: "tArea",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxId",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "MaxIdAlarm",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "SQLqueryAlarm",
                table: "tArea");
        }
    }
}
