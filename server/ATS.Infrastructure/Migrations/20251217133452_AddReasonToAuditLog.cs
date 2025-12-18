using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonToAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ApplicationAuditLogs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ApplicationAuditLogs");
        }
    }
}
