using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class AddedConstraintToEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Properties_EventSetDate_EventClearDate",
                table: "Event",
                sql: "[EventDateTimeClear] > [EventDateTimeSet]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Properties_EventSetDate_EventClearDate",
                table: "Event");
        }
    }
}
