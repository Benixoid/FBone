using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class AddOperatorToNmodeRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LCNId",
                table: "NMTotalRecords",
                newName: "LcnId");

            migrationBuilder.RenameIndex(
                name: "IX_NMTotalRecords_LCNId",
                table: "NMTotalRecords",
                newName: "IX_NMTotalRecords_LcnId");

            migrationBuilder.AddColumn<string>(
                name: "Operator",
                table: "NModeRecords",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true,
                defaultValue: "=");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Operator",
                table: "NModeRecords");

            migrationBuilder.RenameColumn(
                name: "LcnId",
                table: "NMTotalRecords",
                newName: "LCNId");

            migrationBuilder.RenameIndex(
                name: "IX_NMTotalRecords_LcnId",
                table: "NMTotalRecords",
                newName: "IX_NMTotalRecords_LCNId");
        }
    }
}
