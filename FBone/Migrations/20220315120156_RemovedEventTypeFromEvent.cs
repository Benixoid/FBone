using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class RemovedEventTypeFromEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Event");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Event",
                nullable: true);
        }
    }
}
