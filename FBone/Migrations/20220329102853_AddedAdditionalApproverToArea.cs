using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedAdditionalApproverToArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tArea_tPosition_NotifyPos24H",
                table: "tArea");

            migrationBuilder.DropIndex(
                name: "IX_tArea_NotifyPos24H",
                table: "tArea");

            migrationBuilder.AddColumn<int>(
                name: "ApproverAdd",
                table: "tArea",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproverAdd",
                table: "tArea");

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
    }
}
