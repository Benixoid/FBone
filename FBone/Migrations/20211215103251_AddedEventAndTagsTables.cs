using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedEventAndTagsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tagnumber = table.Column<string>(nullable: true),
                    LCN = table.Column<string>(nullable: true),
                    Device = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Service = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TagId = table.Column<int>(nullable: false),
                    EventType = table.Column<string>(nullable: true),
                    ShiftDate = table.Column<DateTime>(nullable: false),
                    ShiftType = table.Column<int>(nullable: false),
                    EventDateTimeSet = table.Column<DateTime>(nullable: false),
                    EventDateTimeClear = table.Column<DateTime>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    DataOrigin = table.Column<string>(nullable: true),
                    ActItemId = table.Column<int>(nullable: false),
                    AddedManually = table.Column<bool>(nullable: false),
                    ReportIt = table.Column<bool>(nullable: false),
                    PSSEventId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_TagId",
                table: "Event",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Tag");
        }
    }
}
