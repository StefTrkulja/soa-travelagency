using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourService.Migrations
{
    /// <inheritdoc />
    public partial class AddTourEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                schema: "tours",
                table: "tours",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DistanceInKm",
                schema: "tours",
                table: "tours",
                type: "numeric(10,3)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                schema: "tours",
                table: "tours",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tour_key_points",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(10,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(11,8)", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_key_points", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tour_key_points_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tour_transport_times",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TransportType = table.Column<string>(type: "text", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour_transport_times", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tour_transport_times_tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tour_key_points_TourId_Order",
                schema: "tours",
                table: "tour_key_points",
                columns: new[] { "TourId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tour_transport_times_TourId_TransportType",
                schema: "tours",
                table: "tour_transport_times",
                columns: new[] { "TourId", "TransportType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tour_key_points",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "tour_transport_times",
                schema: "tours");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                schema: "tours",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "DistanceInKm",
                schema: "tours",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                schema: "tours",
                table: "tours");
        }
    }
}
