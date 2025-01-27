using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class RemovedUnusedFields_3h : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver3h",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "NotifyPos24H",
                table: "tArea");

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
                name: "Flag23h",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "Flag24h",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is3hApproved",
                table: "tAct");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Approver3h",
                table: "tArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NotifyPos24H",
                table: "tArea",
                nullable: true);

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

            migrationBuilder.AddColumn<bool>(
                name: "Flag23h",
                table: "tAct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Flag24h",
                table: "tAct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is3hApproved",
                table: "tAct",
                nullable: false,
                defaultValue: false);
        }
    }
}
