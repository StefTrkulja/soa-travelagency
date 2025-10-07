using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StakeholdersService.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockedFieldToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Blocked",
                schema: "stakeholders",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Blocked",
                schema: "stakeholders",
                table: "Users");
        }
    }
}
