using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedOneToOneEventActItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Event_ActItemId",
                table: "Event",
                column: "ActItemId",
                unique: true,
                filter: "[ActItemId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_tActItems_ActItemId",
                table: "Event",
                column: "ActItemId",
                principalTable: "tActItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_tActItems_ActItemId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_ActItemId",
                table: "Event");
        }
    }
}
