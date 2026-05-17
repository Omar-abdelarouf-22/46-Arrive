using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Arrive.BusApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMwasalatMisr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "Id", "Number" },
                values: new object[] { 3, "M1" });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 21, 30.026, 31.2013, "جامعة القاهرة" },
                    { 22, 30.038399999999999, 31.212299999999999, "ميدان الدقي" },
                    { 23, 30.0444, 31.235700000000001, "ميدان التحرير" },
                    { 24, 30.063400000000001, 31.247199999999999, "ميدان رمسيس" },
                    { 25, 30.0718, 31.282900000000001, "العباسية" },
                    { 26, 30.057099999999998, 31.341100000000001, "مكرم عبيد (مدينة نصر)" },
                    { 27, 30.0335, 31.455500000000001, "الووتر واي (التجمع)" },
                    { 28, 30.016300000000001, 31.5032, "الجامعة الأمريكية (AUC)" }
                });

            migrationBuilder.InsertData(
                table: "BusRoutes",
                columns: new[] { "Id", "BusId", "RouteName" },
                values: new object[] { 3, 3, "خط M1 مواصلات مصر - جامعة القاهرة إلى AUC" });

            migrationBuilder.InsertData(
                table: "RouteStations",
                columns: new[] { "Id", "BusRouteId", "StationId", "StationOrder" },
                values: new object[,]
                {
                    { 26, 3, 21, 1 },
                    { 27, 3, 22, 2 },
                    { 28, 3, 23, 3 },
                    { 29, 3, 24, 4 },
                    { 30, 3, 25, 5 },
                    { 31, 3, 26, 6 },
                    { 32, 3, 27, 7 },
                    { 33, 3, 28, 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "BusRoutes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Buses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.InsertData(
                table: "RouteStations",
                columns: new[] { "Id", "BusRouteId", "StationId", "StationOrder" },
                values: new object[] { 16, 1, 16, 16 });
        }
    }
}
