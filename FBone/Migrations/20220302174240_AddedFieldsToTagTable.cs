using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedFieldsToTagTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Tag",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isFG",
                table: "Tag",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isForBulkInsert",
                table: "Tag",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_DeviceId",
                table: "Tag",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Device_DeviceId",
                table: "Tag",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Device_DeviceId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_DeviceId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "isFG",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "isForBulkInsert",
                table: "Tag");
        }
    }
}
