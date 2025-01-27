using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedNewFieldsToArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotifyOnActCreationEmails",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagForcesActive",
                table: "tArea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyOnActCreationEmails",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "TagForcesActive",
                table: "tArea");
        }
    }
}
