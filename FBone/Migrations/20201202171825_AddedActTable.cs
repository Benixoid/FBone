using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedActTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tAct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    AreaId = table.Column<int>(nullable: false),
                    Crew = table.Column<byte>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    OriginalLang = table.Column<string>(nullable: false, defaultValue: "ru"),
                    CauseRu = table.Column<string>(nullable: true),
                    CauseKz = table.Column<string>(nullable: true),
                    CauseEn = table.Column<string>(nullable: true),
                    ImpactRu = table.Column<string>(nullable: true),
                    ImpactKz = table.Column<string>(nullable: true),
                    ImpactEn = table.Column<string>(nullable: true),
                    ProtectRu = table.Column<string>(nullable: true),
                    ProtectKz = table.Column<string>(nullable: true),
                    ProtectEn = table.Column<string>(nullable: true),
                    ActNotes = table.Column<string>(nullable: true),
                    IsTranslated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tAct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tAct_tArea_AreaId",
                        column: x => x.AreaId,
                        principalTable: "tArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tAct_AreaId",
                table: "tAct",
                column: "AreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tAct");
        }
    }
}
