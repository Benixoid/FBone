using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class AddedCauseImpactProtect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tActCause",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name_EN = table.Column<string>(nullable: true),
                    Name_RU = table.Column<string>(nullable: true),
                    Name_KZ = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tActCause", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tActImpact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name_EN = table.Column<string>(nullable: true),
                    Name_RU = table.Column<string>(nullable: true),
                    Name_KZ = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tActImpact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tActProtect",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name_EN = table.Column<string>(nullable: true),
                    Name_RU = table.Column<string>(nullable: true),
                    Name_KZ = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tActProtect", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tActCause");

            migrationBuilder.DropTable(
                name: "tActImpact");

            migrationBuilder.DropTable(
                name: "tActProtect");
        }
    }
}
