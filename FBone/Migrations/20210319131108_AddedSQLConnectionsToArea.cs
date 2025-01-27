using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedSQLConnectionsToArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "tAct");

            migrationBuilder.RenameColumn(
                name: "Name_Kk",
                table: "tUser",
                newName: "Name_KK");

            migrationBuilder.RenameColumn(
                name: "ProtectRu",
                table: "tAct",
                newName: "ProtectRU");

            migrationBuilder.RenameColumn(
                name: "ProtectKk",
                table: "tAct",
                newName: "ProtectKK");

            migrationBuilder.RenameColumn(
                name: "ProtectEn",
                table: "tAct",
                newName: "ProtectEN");

            migrationBuilder.RenameColumn(
                name: "ImpactRu",
                table: "tAct",
                newName: "ImpactRU");

            migrationBuilder.RenameColumn(
                name: "ImpactKk",
                table: "tAct",
                newName: "ImpactKK");

            migrationBuilder.RenameColumn(
                name: "ImpactEn",
                table: "tAct",
                newName: "ImpactEN");

            migrationBuilder.RenameColumn(
                name: "CauseRu",
                table: "tAct",
                newName: "CauseRU");

            migrationBuilder.RenameColumn(
                name: "CauseKk",
                table: "tAct",
                newName: "CauseKK");

            migrationBuilder.RenameColumn(
                name: "CauseEn",
                table: "tAct",
                newName: "CauseEN");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionString",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EncryptedPassword",
                table: "tArea",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastImportDate",
                table: "tArea",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SQLquery",
                table: "tArea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionString",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "EncryptedPassword",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "LastImportDate",
                table: "tArea");

            migrationBuilder.DropColumn(
                name: "SQLquery",
                table: "tArea");

            migrationBuilder.RenameColumn(
                name: "Name_KK",
                table: "tUser",
                newName: "Name_Kk");

            migrationBuilder.RenameColumn(
                name: "ProtectRU",
                table: "tAct",
                newName: "ProtectRu");

            migrationBuilder.RenameColumn(
                name: "ProtectKK",
                table: "tAct",
                newName: "ProtectKk");

            migrationBuilder.RenameColumn(
                name: "ProtectEN",
                table: "tAct",
                newName: "ProtectEn");

            migrationBuilder.RenameColumn(
                name: "ImpactRU",
                table: "tAct",
                newName: "ImpactRu");

            migrationBuilder.RenameColumn(
                name: "ImpactKK",
                table: "tAct",
                newName: "ImpactKk");

            migrationBuilder.RenameColumn(
                name: "ImpactEN",
                table: "tAct",
                newName: "ImpactEn");

            migrationBuilder.RenameColumn(
                name: "CauseRU",
                table: "tAct",
                newName: "CauseRu");

            migrationBuilder.RenameColumn(
                name: "CauseKK",
                table: "tAct",
                newName: "CauseKk");

            migrationBuilder.RenameColumn(
                name: "CauseEN",
                table: "tAct",
                newName: "CauseEn");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "tAct",
                nullable: false,
                defaultValue: 0);
        }
    }
}
