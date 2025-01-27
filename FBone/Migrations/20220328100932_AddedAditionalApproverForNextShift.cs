using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedAditionalApproverForNextShift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedByAddOn",
                table: "tAct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ApproverAdd",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproverPosAdd",
                table: "tAct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isAddApproved",
                table: "tAct",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByAddOn",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverAdd",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "ApproverPosAdd",
                table: "tAct");

            migrationBuilder.DropColumn(
                name: "isAddApproved",
                table: "tAct");
        }
    }
}
