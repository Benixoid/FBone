using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class ChangesToAreaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "tUser",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tUser_AreaId",
                table: "tUser",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_tUser_tArea_AreaId",
                table: "tUser",
                column: "AreaId",
                principalTable: "tArea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tUser_tArea_AreaId",
                table: "tUser");

            migrationBuilder.DropIndex(
                name: "IX_tUser_AreaId",
                table: "tUser");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "tUser");
        }
    }
}
