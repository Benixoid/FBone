using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedApprovedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "tAct");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedBy1On",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedBy2On",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedBy3On",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedBy4On",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedBy5On",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedBy6On",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedBy7On",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "is1Approved",
                table: "tAct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is2Approved",
                table: "tAct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is3Approved",
                table: "tAct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is4Approved",
                table: "tAct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is5Approved",
                table: "tAct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is6Approved",
                table: "tAct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is7Approved",
                table: "tAct",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy1On",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApprovedBy2On",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApprovedBy3On",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApprovedBy4On",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApprovedBy5On",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApprovedBy6On",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApprovedBy7On",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is1Approved",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is2Approved",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is3Approved",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is4Approved",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is5Approved",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is6Approved",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "is7Approved",
                table: "tAct");

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "tAct",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "tAct",
                nullable: true);
        }
    }
}
