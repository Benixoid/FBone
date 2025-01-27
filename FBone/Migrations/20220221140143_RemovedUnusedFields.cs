using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class RemovedUnusedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LCN",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Event");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LCN",
                table: "Tag",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Event",
                nullable: true);
        }
    }
}
