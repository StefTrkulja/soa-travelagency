using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StakeholdersService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biography",
                schema: "stakeholders",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motto",
                schema: "stakeholders",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "stakeholders",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                schema: "stakeholders",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                schema: "stakeholders",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biography",
                schema: "stakeholders",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Motto",
                schema: "stakeholders",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "stakeholders",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                schema: "stakeholders",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Surname",
                schema: "stakeholders",
                table: "Users");
        }
    }
}
