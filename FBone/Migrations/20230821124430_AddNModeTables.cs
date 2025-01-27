using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class AddNModeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LCNs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LCNs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NModeRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    LcnId = table.Column<int>(type: "int", nullable: false),
                    Tagname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descriptor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountIt = table.Column<bool>(type: "bit", nullable: false),
                    ConditionORed = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    NModeORed = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NModeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NModeRecords_Areas",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NModeRecords_LCNs",
                        column: x => x.LcnId,
                        principalTable: "LCNs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NMTotalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tagname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    LCNId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NMTotalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NMTotalRecords_Areas",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NMTotalRecords_LCNs",
                        column: x => x.LCNId,
                        principalTable: "LCNs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NModeConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NModeRecordId = table.Column<int>(type: "int", nullable: false),
                    Tagname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NModeConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conditions_NModes",
                        column: x => x.NModeRecordId,
                        principalTable: "NModeRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NModeConditions_NModeRecordId",
                table: "NModeConditions",
                column: "NModeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_NModeRecords_AreaId",
                table: "NModeRecords",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_NModeRecords_LcnId",
                table: "NModeRecords",
                column: "LcnId");

            migrationBuilder.CreateIndex(
                name: "IX_NMTotalRecords_AreaId",
                table: "NMTotalRecords",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_NMTotalRecords_LCNId",
                table: "NMTotalRecords",
                column: "LCNId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NModeConditions");

            migrationBuilder.DropTable(
                name: "NMTotalRecords");

            migrationBuilder.DropTable(
                name: "NModeRecords");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "LCNs");
        }
    }
}
