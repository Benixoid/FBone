using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedLinkBetweenTagAndArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Tag",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_AreaId",
                table: "Tag",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_tArea_AreaId",
                table: "Tag",
                column: "AreaId",
                principalTable: "tArea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_tArea_AreaId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_AreaId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Tag");
        }
    }
}
