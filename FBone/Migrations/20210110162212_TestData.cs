using Microsoft.EntityFrameworkCore.Migrations;

namespace FBone.Migrations
{
    public partial class TestData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("set identity_insert dbo.tposition on");
            migrationBuilder.Sql("INSERT [dbo].[tPosition] ([Id], [Name], [CanCreateAct], [CanTranslateAct], [IsShiftEngineer]) VALUES (1, N'Autamation DB engineer', 1, 0, 0)");
            migrationBuilder.Sql("INSERT [dbo].[tPosition] ([Id], [Name], [CanCreateAct], [CanTranslateAct], [IsShiftEngineer]) VALUES(2, N'Autamation PI engineer', 1, 0, 0)");

            migrationBuilder.Sql("INSERT [dbo].[tUser] ([CAI], [Name_EN], [Name_RU], [Name_KZ], [IsAdmin], [IsActive], [Email], [lang], [PositionId]) VALUES (N'SB-LPT470\\Admin', N'Sabirov Baurzhan', N'Сабиров Бауржан', N'Сабиров Бауржан', 1, 1, N'ben.evans@mail.ru', N'ru', 1)");
            migrationBuilder.Sql("INSERT [dbo].[tUser] ([CAI], [Name_EN], [Name_RU], [Name_KZ], [IsAdmin], [IsActive], [Email], [lang], [PositionId]) VALUES (N'ct\\bsqo', N'Sabirov Baurzhan', N'Сабиров Бауржан', N'Сабиров Бауржан', 1, 1, N'bsqo@tengizchevroil.com', N'en', 1)");
            migrationBuilder.Sql("INSERT [dbo].[tUser] ([CAI], [Name_EN], [Name_RU], [Name_KZ], [IsAdmin], [IsActive], [Email], [lang], [PositionId]) VALUES (N'ct\\begc', N'Begaliev', N'Бегалиев', N'бегалиев', 1, 1, N'begc@tengizchevroil.com', N'en', 1)");
            migrationBuilder.Sql("INSERT [dbo].[tUser] ([CAI], [Name_EN], [Name_RU], [Name_KZ], [IsAdmin], [IsActive], [Email], [lang], [PositionId]) VALUES (N'ct\\akxa', N'Bergaliev', N'Бергалиев', N'бергалиев', 1, 1, N'akxa@tengizchevroil.com', N'kk', 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
