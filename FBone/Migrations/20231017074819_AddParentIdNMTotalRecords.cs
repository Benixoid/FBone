using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class AddParentIdNMTotalRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "NMTotalRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NMTotalRecords_ParentId",
                table: "NMTotalRecords",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_NMTotalRecords_NMTotalRecords",
                table: "NMTotalRecords",
                column: "ParentId",
                principalTable: "NMTotalRecords",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NMTotalRecords_NMTotalRecords",
                table: "NMTotalRecords");

            migrationBuilder.DropIndex(
                name: "IX_NMTotalRecords_ParentId",
                table: "NMTotalRecords");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "NMTotalRecords");
        }
    }
}
