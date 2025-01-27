using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class CreateNModeUpdatetPositionAreas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNModeAdministrator",
                table: "tPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InterpolatedValuesCount",
                table: "Areas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "SplitToShift",
                table: "Areas",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "StartingHour",
                table: "Areas",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "NModeResults",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: false),
                    NormalTotal = table.Column<double>(type: "float", nullable: false),
                    DayNormal = table.Column<double>(type: "float", nullable: false),
                    DayManual = table.Column<double>(type: "float", nullable: false),
                    DayOther = table.Column<double>(type: "float", nullable: false),
                    NightNormal = table.Column<double>(type: "float", nullable: false),
                    NightManual = table.Column<double>(type: "float", nullable: false),
                    NightOther = table.Column<double>(type: "float", nullable: false),
                    RecordTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Evaluation = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NModeResults", x => x.UID);
                    table.ForeignKey(
                        name: "FK_NModeResults_NModeRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "NModeRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NModeResults_RecordId",
                table: "NModeResults",
                column: "RecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NModeResults");

            migrationBuilder.DropColumn(
                name: "IsNModeAdministrator",
                table: "tPosition");

            migrationBuilder.DropColumn(
                name: "InterpolatedValuesCount",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "SplitToShift",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "StartingHour",
                table: "Areas");
        }
    }
}
