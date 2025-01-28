using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    /// <inheritdoc />
    public partial class AddedauditVerificatorPositionToArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VerificatorId",
                table: "tArea",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tArea_VerificatorId",
                table: "tArea",
                column: "VerificatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_tArea_tPosition_VerificatorId",
                table: "tArea",
                column: "VerificatorId",
                principalTable: "tPosition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tArea_tPosition_VerificatorId",
                table: "tArea");

            migrationBuilder.DropIndex(
                name: "IX_tArea_VerificatorId",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "VerificatorId",
                table: "tArea");
        }
    }
}
