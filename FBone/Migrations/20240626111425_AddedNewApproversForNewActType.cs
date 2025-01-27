using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewApproversForNewActType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Approver5_1",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver5_2",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver5_3",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver5_4",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver5_5",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver5_6",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver5_7",
                table: "tArea",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver5_1",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver5_2",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver5_3",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver5_4",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver5_5",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver5_6",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver5_7",
                table: "tArea");
        }
    }
}
