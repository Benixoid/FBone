using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedApprovesToAct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Approver1",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver2",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver3",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver4",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver5",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver6",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Approver7",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPos1",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPos2",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPos3",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPos4",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPos5",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPos6",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPos7",
                table: "tAct",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver1",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "Approver2",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "Approver3",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "Approver4",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "Approver5",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "Approver6",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "Approver7",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPos1",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPos2",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPos3",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPos4",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPos5",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPos6",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPos7",
                table: "tAct");
        }
    }
}
