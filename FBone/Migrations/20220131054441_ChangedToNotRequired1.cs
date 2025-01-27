using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class ChangedToNotRequired1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ActItemId",
                table: "Event",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ActItemId",
                table: "Event",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
