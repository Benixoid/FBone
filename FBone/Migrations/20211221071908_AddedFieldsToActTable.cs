using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedFieldsToActTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "tAct",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "tAct",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "tAct");
        }
    }
}
