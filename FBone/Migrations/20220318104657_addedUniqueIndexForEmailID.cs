using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class addedUniqueIndexForEmailID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmailId",
                table: "EmailTemplate",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "Index_EmailId",
                table: "EmailTemplate",
                column: "EmailId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Index_EmailId",
                table: "EmailTemplate");

            migrationBuilder.AlterColumn<string>(
                name: "EmailId",
                table: "EmailTemplate",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
