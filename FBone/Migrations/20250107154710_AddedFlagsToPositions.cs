using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    /// <inheritdoc />
    public partial class AddedFlagsToPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "tPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAuditCreator",
                table: "tPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "Approvers5Disabled",
            //    table: "tArea",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "tPosition");

            migrationBuilder.DropColumn(
                name: "IsAuditCreator",
                table: "tPosition");

            //migrationBuilder.DropColumn(
            //    name: "Approvers5Disabled",
            //    table: "tArea");
        }
    }
}
