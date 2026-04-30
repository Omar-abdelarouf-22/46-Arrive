using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Arrive.BusApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMetroStations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetroStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetroStations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MetroStations",
                columns: new[] { "Id", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, 29.8489, 31.334199999999999, "Helwan" },
                    { 2, 29.9602, 31.2576, "Maadi" },
                    { 3, 30.0444, 31.235800000000001, "Sadat" },
                    { 4, 30.0535, 31.238700000000001, "Nasser" },
                    { 5, 30.061, 31.245999999999999, "Al Shohadaa" },
                    { 6, 30.052299999999999, 31.2468, "Attaba" },
                    { 7, 30.026, 31.2013, "Cairo University" },
                    { 8, 30.038499999999999, 31.2121, "Dokki" },
                    { 9, 30.041899999999998, 31.224900000000002, "Opera" },
                    { 10, 30.073499999999999, 31.283100000000001, "Abbassia" },
                    { 11, 30.101500000000001, 31.333100000000002, "Haroun" },
                    { 12, 30.152100000000001, 31.3384, "El Marg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetroStations");
        }
    }
}
