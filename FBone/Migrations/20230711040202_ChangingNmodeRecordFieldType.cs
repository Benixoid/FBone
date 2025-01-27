using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    public partial class ChangingNmodeRecordFieldType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NModeID",
                table: "NModeRecords",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                table: "NModeConditions",
                newName: "NModeRecordId");

            migrationBuilder.RenameColumn(
                name: "ConditionID",
                table: "NModeConditions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_NModeConditions_ParentID",
                table: "NModeConditions",
                newName: "IX_NModeConditions_NModeRecordId");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Descriptor",
            //    table: "NModeRecords",
            //    type: "nvarchar(50)",
            //    maxLength: 50,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(50)",
            //    oldMaxLength: 50);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Value",
            //    table: "NModeConditions",
            //    type: "nvarchar(10)",
            //    maxLength: 10,
            //    nullable: false,
            //    oldClrType: typeof(object),
            //    oldType: "sql_variant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "NModeRecords",
                newName: "NModeID");

            migrationBuilder.RenameColumn(
                name: "NModeRecordId",
                table: "NModeConditions",
                newName: "ParentID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "NModeConditions",
                newName: "ConditionID");

            migrationBuilder.RenameIndex(
                name: "IX_NModeConditions_NModeRecordId",
                table: "NModeConditions",
                newName: "IX_NModeConditions_ParentID");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptor",
                table: "NModeRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<object>(
                name: "Value",
                table: "NModeConditions",
                type: "sql_variant",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }
    }
}
