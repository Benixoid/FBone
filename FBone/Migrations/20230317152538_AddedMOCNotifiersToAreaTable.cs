using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class AddedMOCNotifiersToAreaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotifyForMocInitiate",
                table: "tArea",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyForMocInitiate",
                table: "tArea");
        }
    }
}
