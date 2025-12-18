using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS.Infrastructure.Migrations
{
    public partial class AddMultiRecruiterOwnership : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Safely drop RecruiterId ONLY if it exists
            migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM information_schema.columns
                    WHERE table_name = 'Applications'
                    AND column_name = 'RecruiterId'
                ) THEN
                    ALTER TABLE "Applications" DROP COLUMN "RecruiterId";
                END IF;
            END$$;
            """);

            migrationBuilder.CreateTable(
                name: "JobApplicationAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecruiterId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplicationAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplicationAssignments_Applications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplicationAssignments_Recruiters_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Recruiters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationAssignments_JobApplicationId",
                table: "JobApplicationAssignments",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationAssignments_RecruiterId",
                table: "JobApplicationAssignments",
                column: "RecruiterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobApplicationAssignments");

            migrationBuilder.AddColumn<Guid>(
                name: "RecruiterId",
                table: "Applications",
                type: "uuid",
                nullable: true);
        }
    }
}
