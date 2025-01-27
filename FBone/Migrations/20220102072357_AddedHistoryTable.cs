using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActId = table.Column<int>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActHistory_tAct_ActId",
                        column: x => x.ActId,
                        principalTable: "tAct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActHistory_tUser_UserId",
                        column: x => x.UserId,
                        principalTable: "tUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActHistory_ActId",
                table: "ActHistory",
                column: "ActId");

            migrationBuilder.CreateIndex(
                name: "IX_ActHistory_UserId",
                table: "ActHistory",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActHistory");
        }
    }
}
