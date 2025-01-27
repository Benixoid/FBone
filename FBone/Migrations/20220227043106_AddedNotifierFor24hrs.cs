using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedNotifierFor24hrs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotifyPos24H",
                table: "tArea",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tArea_NotifyPos24H",
                table: "tArea",
                column: "NotifyPos24H");

            migrationBuilder.AddForeignKey(
                name: "FK_tArea_tPosition_NotifyPos24H",
                table: "tArea",
                column: "NotifyPos24H",
                principalTable: "tPosition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tArea_tPosition_NotifyPos24H",
                table: "tArea");

            migrationBuilder.DropIndex(
                name: "IX_tArea_NotifyPos24H",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "NotifyPos24H",
                table: "tArea");
        }
    }
}
