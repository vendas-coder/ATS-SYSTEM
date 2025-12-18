using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    OldStatus = table.Column<int>(type: "integer", nullable: true),
                    NewStatus = table.Column<int>(type: "integer", nullable: true),
                    RecruiterId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationAuditLogs_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationAuditLogs_Applications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationAuditLogs_ApplicationId",
                table: "ApplicationAuditLogs",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationAuditLogs_JobApplicationId",
                table: "ApplicationAuditLogs",
                column: "JobApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationAuditLogs");
        }
    }
}
