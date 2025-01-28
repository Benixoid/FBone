using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    /// <inheritdoc />
    public partial class AuditAddedsecondApprover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupervisorNote",
                table: "Audits",
                newName: "Approval2Note");

            migrationBuilder.AddColumn<string>(
                name: "Approval1Note",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approval1Note",
                table: "Audits");

            migrationBuilder.RenameColumn(
                name: "Approval2Note",
                table: "Audits",
                newName: "SupervisorNote");
        }
    }
}
