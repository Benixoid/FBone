using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class ChangedFieldNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ForceSetTime",
                table: "tActItems",
                newName: "SetTime");

            migrationBuilder.RenameColumn(
                name: "ForceClearTime",
                table: "tActItems",
                newName: "ClearTime");

            migrationBuilder.AddColumn<int>(
                name: "Device",
                table: "tActItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Device",
                table: "tActItems");

            migrationBuilder.RenameColumn(
                name: "SetTime",
                table: "tActItems",
                newName: "ForceSetTime");

            migrationBuilder.RenameColumn(
                name: "ClearTime",
                table: "tActItems",
                newName: "ForceClearTime");
        }
    }
}
