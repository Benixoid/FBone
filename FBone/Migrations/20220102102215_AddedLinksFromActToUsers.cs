using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedLinksFromActToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClosedByUserId",
                table: "tAct",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tAct_ClosedByUserId",
                table: "tAct",
                column: "ClosedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_tAct_CreatedByUserId",
                table: "tAct",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tAct_tUser_ClosedByUserId",
                table: "tAct",
                column: "ClosedByUserId",
                principalTable: "tUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tAct_tUser_CreatedByUserId",
                table: "tAct",
                column: "CreatedByUserId",
                principalTable: "tUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tAct_tUser_ClosedByUserId",
                table: "tAct");

            migrationBuilder.DropForeignKey(
                name: "FK_tAct_tUser_CreatedByUserId",
                table: "tAct");

            migrationBuilder.DropIndex(
                name: "IX_tAct_ClosedByUserId",
                table: "tAct");

            migrationBuilder.DropIndex(
                name: "IX_tAct_CreatedByUserId",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ClosedByUserId",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "tAct");
        }
    }
}
