using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiRecruiterOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationNotes_Applications_JobApplicationId1",
                table: "ApplicationNotes");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationNotes_JobApplicationId1",
                table: "ApplicationNotes");

            migrationBuilder.DropColumn(
                name: "RecruiterId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "JobApplicationId1",
                table: "ApplicationNotes");

            migrationBuilder.AddColumn<Guid>(
                name: "NewRecruiterId",
                table: "ApplicationEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OldRecruiterId",
                table: "ApplicationEvents",
                type: "uuid",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobApplicationAssignments");

            migrationBuilder.DropColumn(
                name: "NewRecruiterId",
                table: "ApplicationEvents");

            migrationBuilder.DropColumn(
                name: "OldRecruiterId",
                table: "ApplicationEvents");

            migrationBuilder.AddColumn<Guid>(
                name: "RecruiterId",
                table: "Applications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "JobApplicationId1",
                table: "ApplicationNotes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationNotes_JobApplicationId1",
                table: "ApplicationNotes",
                column: "JobApplicationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationNotes_Applications_JobApplicationId1",
                table: "ApplicationNotes",
                column: "JobApplicationId1",
                principalTable: "Applications",
                principalColumn: "Id");
        }
    }
}
