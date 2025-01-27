using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedPositionToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "tUser",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tUser_PositionId",
                table: "tUser",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_tUser_tPosition_PositionId",
                table: "tUser",
                column: "PositionId",
                principalTable: "tPosition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tUser_tPosition_PositionId",
                table: "tUser");

            migrationBuilder.DropIndex(
                name: "IX_tUser_PositionId",
                table: "tUser");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "tUser");
        }
    }
}
