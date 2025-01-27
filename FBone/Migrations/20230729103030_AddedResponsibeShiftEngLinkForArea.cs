using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class AddedResponsibeShiftEngLinkForArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[tArea] set ShiftEngFacilityId=1");
            migrationBuilder.CreateIndex(
                name: "IX_tArea_ShiftEngFacilityId",
                table: "tArea",
                column: "ShiftEngFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_tArea_tFacility_ShiftEngFacilityId",
                table: "tArea",
                column: "ShiftEngFacilityId",
                principalTable: "tFacility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tArea_tFacility_ShiftEngFacilityId",
                table: "tArea");

            migrationBuilder.DropIndex(
                name: "IX_tArea_ShiftEngFacilityId",
                table: "tArea");
        }
    }
}
