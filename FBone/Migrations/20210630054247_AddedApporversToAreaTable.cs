using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedApporversToAreaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Approver1_1",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver1_2",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver1_3",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver1_4",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver1_5",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver1_6",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver1_7",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver2_1",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver2_2",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver2_3",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver2_4",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver2_5",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver2_6",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver2_7",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver3_1",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver3_2",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver3_3",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver3_4",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver3_5",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver3_6",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver3_7",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver4_1",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver4_2",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver4_3",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver4_4",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver4_5",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver4_6",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver4_7",
                table: "tArea",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver1_1",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver1_2",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver1_3",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver1_4",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver1_5",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver1_6",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver1_7",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver2_1",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver2_2",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver2_3",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver2_4",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver2_5",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver2_6",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver2_7",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver3_1",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver3_2",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver3_3",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver3_4",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver3_5",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver3_6",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver3_7",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver4_1",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver4_2",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver4_3",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver4_4",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver4_5",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver4_6",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "Approver4_7",
                table: "tArea");
        }
    }
}
