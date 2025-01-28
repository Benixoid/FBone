using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    /// <inheritdoc />
    public partial class AuditAddedAdditionalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActionCompletedOn",
                table: "Audits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Approved1On",
                table: "Audits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Approved2On",
                table: "Audits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedOn",
                table: "Audits",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionCompletedOn",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "Approved1On",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "Approved2On",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "VerifiedOn",
                table: "Audits");
        }
    }
}
