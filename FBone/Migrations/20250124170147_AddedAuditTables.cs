using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBone.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuditTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    ShiftDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActId = table.Column<int>(type: "int", nullable: false),
                    RequiredActionNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionTakenNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupervisorNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionOwnerPositionId = table.Column<int>(type: "int", nullable: false),
                    CompletedByUserId = table.Column<int>(type: "int", nullable: true),
                    VerifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    Approved1ByUserId = table.Column<int>(type: "int", nullable: true),
                    Approved2ByUserId = table.Column<int>(type: "int", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audits_tAct_ActId",
                        column: x => x.ActId,
                        principalTable: "tAct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Audits_tArea_AreaId",
                        column: x => x.AreaId,
                        principalTable: "tArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Audits_tFacility_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "tFacility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Audits_tPosition_ActionOwnerPositionId",
                        column: x => x.ActionOwnerPositionId,
                        principalTable: "tPosition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Audits_tUser_Approved1ByUserId",
                        column: x => x.Approved1ByUserId,
                        principalTable: "tUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audits_tUser_Approved2ByUserId",
                        column: x => x.Approved2ByUserId,
                        principalTable: "tUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audits_tUser_CompletedByUserId",
                        column: x => x.CompletedByUserId,
                        principalTable: "tUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audits_tUser_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "tUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Audits_tUser_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "tUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuditFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    File = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditFiles_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditFiles_AuditId",
                table: "AuditFiles",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_ActId",
                table: "Audits",
                column: "ActId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_ActionOwnerPositionId",
                table: "Audits",
                column: "ActionOwnerPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_Approved1ByUserId",
                table: "Audits",
                column: "Approved1ByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_Approved2ByUserId",
                table: "Audits",
                column: "Approved2ByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_AreaId",
                table: "Audits",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_CompletedByUserId",
                table: "Audits",
                column: "CompletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_CreatedByUserID",
                table: "Audits",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_FacilityId",
                table: "Audits",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_VerifiedByUserId",
                table: "Audits",
                column: "VerifiedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditFiles");

            migrationBuilder.DropTable(
                name: "Audits");
        }
    }
}
