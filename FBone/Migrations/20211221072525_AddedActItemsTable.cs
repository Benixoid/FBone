using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedActItemsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tActItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActId = table.Column<int>(nullable: false),
                    TagName = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true),
                    Equipment = table.Column<string>(nullable: true),
                    ForceSetTime = table.Column<DateTime>(nullable: false),
                    ForceClearTime = table.Column<DateTime>(nullable: false),
                    Action = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tActItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tActItems_tAct_ActId",
                        column: x => x.ActId,
                        principalTable: "tAct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tActItems_ActId",
                table: "tActItems",
                column: "ActId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tActItems");
        }
    }
}
