using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedLastRecordIdToAreaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastRecordId",
                table: "tArea",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRecordId",
                table: "tArea");
        }
    }
}
