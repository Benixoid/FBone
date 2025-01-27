﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class RemoveNModeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NModeConditions");

            migrationBuilder.DropTable(
                name: "NModeRecords");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NModeRecords",
                columns: table => new
                {
                    NModeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConditionORed = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CountIt = table.Column<bool>(type: "bit", nullable: false),
                    Descriptor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LCN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tagname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NModeTable", x => x.NModeID);
                });

            migrationBuilder.CreateTable(
                name: "NModeConditions",
                columns: table => new
                {
                    ConditionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentID = table.Column<int>(type: "int", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Tagname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<object>(type: "sql_variant", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditions", x => x.ConditionID);
                    table.ForeignKey(
                        name: "FK_Conditions_NModes",
                        column: x => x.ParentID,
                        principalTable: "NModeRecords",
                        principalColumn: "NModeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NModeConditions_ParentID",
                table: "NModeConditions",
                column: "ParentID");
        }
    }
}
