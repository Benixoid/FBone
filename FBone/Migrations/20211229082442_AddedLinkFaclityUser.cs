using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedLinkFaclityUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tUser_FacilityId",
                table: "tUser",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_tUser_tFacility_FacilityId",
                table: "tUser",
                column: "FacilityId",
                principalTable: "tFacility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tUser_tFacility_FacilityId",
                table: "tUser");

            migrationBuilder.DropIndex(
                name: "IX_tUser_FacilityId",
                table: "tUser");
        }
    }
}
