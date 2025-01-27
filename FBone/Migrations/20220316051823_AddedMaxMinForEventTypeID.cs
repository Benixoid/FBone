using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedMaxMinForEventTypeID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Event ADD CONSTRAINT CK_TypeId_check CHECK (TypeId > 0 and TypeId < 5)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
