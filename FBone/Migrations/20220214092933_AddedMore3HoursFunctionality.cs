using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedMore3HoursFunctionality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedBy3hOn",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Approver3h",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPos3h",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedOn",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "is3hApproved",
                table: "tAct",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy3hOn",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "Approver3h",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPos3h",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "StartedOn",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is3hApproved",
                table: "tAct");
        }
    }
}
