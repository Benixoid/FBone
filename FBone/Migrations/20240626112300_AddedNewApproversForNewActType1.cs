using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewApproversForNewActType1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotifyOnType5ActApproved",
                table: "tArea",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyOnType5ActApproved",
                table: "tArea");
        }
    }
}
