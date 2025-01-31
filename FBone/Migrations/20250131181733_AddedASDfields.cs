using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    /// <inheritdoc />
    public partial class AddedASDfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AsdApproverId",
                table: "tArea",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isASD",
                table: "Tag",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_tArea_AsdApproverId",
                table: "tArea",
                column: "AsdApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_tArea_tPosition_AsdApproverId",
                table: "tArea",
                column: "AsdApproverId",
                principalTable: "tPosition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tArea_tPosition_AsdApproverId",
                table: "tArea");

            migrationBuilder.DropIndex(
                name: "IX_tArea_AsdApproverId",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "AsdApproverId",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "isASD",
                table: "Tag");
        }
    }
}
