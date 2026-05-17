using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Arrive.BusApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedBusRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "Id", "Number" },
                values: new object[,]
                {
                    { 1, "80" },
                    { 2, "90" }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, 30.136900000000001, 31.3217, "مساكن اسكو" },
                    { 2, 30.140699999999999, 31.296800000000001, "بهتيم" },
                    { 3, 30.129799999999999, 31.3095, "الشارع الجديد" },
                    { 4, 30.1389, 31.293500000000002, "مسطرد" },
                    { 5, 30.117100000000001, 31.313099999999999, "المطرية" },
                    { 6, 30.095700000000001, 31.287600000000001, "الحلمية" },
                    { 7, 30.0867, 31.307500000000001, "التجنيد" },
                    { 8, 30.084099999999999, 31.3032, "محكمة" },
                    { 9, 30.091100000000001, 31.319700000000001, "روكسي" },
                    { 10, 30.089099999999998, 31.342099999999999, "ارض الجولف" },
                    { 11, 30.079699999999999, 31.298200000000001, "الخليفة المأمون" },
                    { 12, 30.0871, 31.294599999999999, "كوبري القبة" },
                    { 13, 30.086400000000001, 31.284099999999999, "حدائق القبة" },
                    { 14, 30.081900000000001, 31.278199999999998, "حمامات القبة" },
                    { 15, 30.075399999999998, 31.274899999999999, "منشية الصدر" },
                    { 16, 30.070699999999999, 31.271699999999999, "مستشفى الدمرداش" },
                    { 17, 30.1327, 31.300899999999999, "عرب" },
                    { 18, 30.061399999999999, 31.245799999999999, "مستشفى الجلاء" },
                    { 19, 30.061900000000001, 31.246500000000001, "شارع رمسيس" },
                    { 20, 30.0106, 31.208600000000001, "ميدان الجيزة" }
                });

            migrationBuilder.InsertData(
                table: "BusRoutes",
                columns: new[] { "Id", "BusId", "RouteName" },
                values: new object[,]
                {
                    { 1, 1, "خط 80 - مساكن اسكو إلى مستشفى الدمرداش" },
                    { 2, 2, "خط 90 - مساكن اسكو إلى ميدان الجيزة" }
                });

            migrationBuilder.InsertData(
                table: "RouteStations",
                columns: new[] { "Id", "BusRouteId", "StationId", "StationOrder" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 1, 2, 2 },
                    { 3, 1, 3, 3 },
                    { 4, 1, 4, 4 },
                    { 5, 1, 5, 5 },
                    { 6, 1, 6, 6 },
                    { 7, 1, 7, 7 },
                    { 8, 1, 8, 8 },
                    { 9, 1, 9, 9 },
                    { 10, 1, 10, 10 },
                    { 11, 1, 11, 11 },
                    { 12, 1, 12, 12 },
                    { 13, 1, 13, 13 },
                    { 14, 1, 14, 14 },
                    { 15, 1, 15, 15 },
                    { 16, 1, 16, 16 },
                    { 17, 2, 1, 1 },
                    { 18, 2, 2, 2 },
                    { 19, 2, 17, 3 },
                    { 20, 2, 4, 4 },
                    { 21, 2, 5, 5 },
                    { 22, 2, 9, 6 },
                    { 23, 2, 18, 7 },
                    { 24, 2, 19, 8 },
                    { 25, 2, 20, 9 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "RouteStations",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "BusRoutes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BusRoutes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Buses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Buses",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
