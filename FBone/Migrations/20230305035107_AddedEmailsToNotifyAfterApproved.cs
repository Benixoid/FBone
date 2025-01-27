using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class AddedEmailsToNotifyAfterApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotifyOn2oo3ActApproved",
                table: "tArea",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotifyOnBypassActApproved",
                table: "tArea",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotifyOnForceActApproved",
                table: "tArea",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotifyOnInactiveActApproved",
                table: "tArea",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyOn2oo3ActApproved",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "NotifyOnBypassActApproved",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "NotifyOnForceActApproved",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "NotifyOnInactiveActApproved",
                table: "tArea");
        }
    }
}
