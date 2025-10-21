using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StakeholdersService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                schema: "stakeholders",
                table: "Users",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LocationUpdatedAt",
                schema: "stakeholders",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                schema: "stakeholders",
                table: "Users",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "stakeholders",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LocationUpdatedAt",
                schema: "stakeholders",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "stakeholders",
                table: "Users");
        }
    }
}
