using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class TemporaryAddedEventTypeToEventTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Event",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Event");
        }
    }
}
