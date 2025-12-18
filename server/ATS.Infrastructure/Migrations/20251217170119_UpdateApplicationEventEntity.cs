using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "ApplicationEvents",
                newName: "Actor");

            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "ApplicationEvents",
                newName: "ActionType");

            migrationBuilder.AddColumn<int>(
                name: "NewStatus",
                table: "ApplicationEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldStatus",
                table: "ApplicationEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ApplicationEvents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "ApplicationEvents");

            migrationBuilder.DropColumn(
                name: "OldStatus",
                table: "ApplicationEvents");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ApplicationEvents");

            migrationBuilder.RenameColumn(
                name: "Actor",
                table: "ApplicationEvents",
                newName: "Payload");

            migrationBuilder.RenameColumn(
                name: "ActionType",
                table: "ApplicationEvents",
                newName: "EventType");
        }
    }
}
