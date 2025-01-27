using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedLinkFaclityArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tArea_FacilityId",
                table: "tArea",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_tArea_tFacility_FacilityId",
                table: "tArea",
                column: "FacilityId",
                principalTable: "tFacility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tArea_tFacility_FacilityId",
                table: "tArea");

            migrationBuilder.DropIndex(
                name: "IX_tArea_FacilityId",
                table: "tArea");
        }
    }
}
