using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Arrive.BusApi.Migrations
{
    /// <inheritdoc />
    public partial class CleanSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusRoutes_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusRouteId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    StationOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteStations_BusRoutes_BusRouteId",
                        column: x => x.BusRouteId,
                        principalTable: "BusRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteStations_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MetroStations",
                columns: new[] { "Id", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, 29.8491, 31.334099999999999, "حلوان" },
                    { 2, 29.864100000000001, 31.3323, "عين حلوان" },
                    { 3, 29.8706, 31.328299999999999, "جامعة حلوان" },
                    { 4, 29.882100000000001, 31.3218, "وادي حوف" },
                    { 5, 29.897200000000002, 31.312899999999999, "حدائق حلوان" },
                    { 6, 29.907900000000001, 31.303899999999999, "المعصرة" },
                    { 7, 29.924800000000001, 31.291699999999999, "طرة الأسمنت" },
                    { 8, 29.936599999999999, 31.282900000000001, "كوتسيكا" },
                    { 9, 29.9482, 31.274999999999999, "طرة البلد" },
                    { 10, 29.954599999999999, 31.265699999999999, "ثكنات المعادي" },
                    { 11, 29.9602, 31.258500000000002, "المعادي" },
                    { 12, 29.972100000000001, 31.250499999999999, "حدائق المعادي" },
                    { 13, 29.982199999999999, 31.242999999999999, "دار السلام" },
                    { 14, 29.995899999999999, 31.233499999999999, "الزهراء" },
                    { 15, 30.0062, 31.23, "مار جرجس" },
                    { 16, 30.017499999999998, 31.229099999999999, "الملك الصالح" },
                    { 17, 30.029599999999999, 31.2332, "السيدة زينب" },
                    { 18, 30.035699999999999, 31.236599999999999, "سعد زغلول" },
                    { 19, 30.0444, 31.235800000000001, "السادات" },
                    { 20, 30.0535, 31.238700000000001, "جمال عبد الناصر" },
                    { 21, 30.0579, 31.242699999999999, "عرابي" },
                    { 22, 30.061, 31.245999999999999, "الشهداء" },
                    { 23, 30.066400000000002, 31.264399999999998, "غمرة" },
                    { 24, 30.0715, 31.2727, "الدمرداش" },
                    { 25, 30.075900000000001, 31.276599999999998, "منشية الصدر" },
                    { 26, 30.081800000000001, 31.284500000000001, "كوبري القبة" },
                    { 27, 30.0854, 31.290500000000002, "حمامات القبة" },
                    { 28, 30.0898, 31.296500000000002, "سراي القبة" },
                    { 29, 30.098800000000001, 31.306899999999999, "حدائق الزيتون" },
                    { 30, 30.104399999999998, 31.3125, "حلمية الزيتون" },
                    { 31, 30.114699999999999, 31.318100000000001, "المطرية" },
                    { 32, 30.124300000000002, 31.323499999999999, "عين شمس" },
                    { 33, 30.1387, 31.328299999999999, "عزبة النخل" },
                    { 34, 30.152100000000001, 31.3384, "المرج" },
                    { 35, 30.1633, 31.336400000000001, "المرج الجديدة" },
                    { 36, 29.980599999999999, 31.211400000000001, "المنيب" },
                    { 37, 29.995799999999999, 31.209800000000001, "ساقية مكي" },
                    { 38, 30.005099999999999, 31.2089, "أم المصريين" },
                    { 39, 30.0137, 31.207899999999999, "الجيزة" },
                    { 40, 30.021000000000001, 31.204499999999999, "فيصل" },
                    { 41, 30.026, 31.2013, "جامعة القاهرة" },
                    { 42, 30.035499999999999, 31.200099999999999, "البحوث" },
                    { 43, 30.038499999999999, 31.2121, "الدقي" },
                    { 44, 30.041899999999998, 31.224900000000002, "الأوبرا" },
                    { 45, 30.0443, 31.244299999999999, "محمد نجيب" },
                    { 46, 30.052299999999999, 31.2468, "العتبة" },
                    { 47, 30.070900000000002, 31.245200000000001, "مسرة" },
                    { 48, 30.081399999999999, 31.246400000000001, "روض الفرج" },
                    { 49, 30.091000000000001, 31.2469, "سانت تريزا" },
                    { 50, 30.100899999999999, 31.247499999999999, "الخلفاوي" },
                    { 51, 30.106999999999999, 31.249199999999998, "المظلات" },
                    { 52, 30.113299999999999, 31.2486, "كلية الزراعة" },
                    { 53, 30.122499999999999, 31.244700000000002, "شبرا الخيمة" },
                    { 54, 30.1465, 31.421700000000001, "عدلي منصور" },
                    { 55, 30.135999999999999, 31.4055, "الهايكستب" },
                    { 56, 30.130199999999999, 31.3855, "عمر بن الخطاب" },
                    { 57, 30.1265, 31.3705, "قباء" },
                    { 58, 30.122499999999999, 31.355499999999999, "هشام بركات" },
                    { 59, 30.118500000000001, 31.345500000000001, "النزهة" },
                    { 60, 30.1145, 31.3355, "نادي الشمس" },
                    { 61, 30.109500000000001, 31.325500000000002, "ألف مسكن" },
                    { 62, 30.1005, 31.320499999999999, "ميدان هليوبوليس" },
                    { 63, 30.093499999999999, 31.3155, "هارون" },
                    { 64, 30.0885, 31.3125, "الأهرام" },
                    { 65, 30.083500000000001, 31.3095, "كلية البنات" },
                    { 66, 30.073499999999999, 31.3065, "الاستاد" },
                    { 67, 30.070499999999999, 31.298500000000001, "أرض المعارض" },
                    { 68, 30.073499999999999, 31.283100000000001, "العباسية" },
                    { 69, 30.0655, 31.275500000000001, "عبده باشا" },
                    { 70, 30.060500000000001, 31.265499999999999, "الجيش" },
                    { 71, 30.055499999999999, 31.255500000000001, "باب الشعرية" },
                    { 72, 30.055099999999999, 31.232500000000002, "ماسبيرو" },
                    { 73, 30.0625, 31.2225, "صفاء حجازي" },
                    { 74, 30.0685, 31.212499999999999, "الكيت كات" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusRoutes_BusId",
                table: "BusRoutes",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStations_BusRouteId_StationId",
                table: "RouteStations",
                columns: new[] { "BusRouteId", "StationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStations_BusRouteId_StationOrder",
                table: "RouteStations",
                columns: new[] { "BusRouteId", "StationOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStations_StationId",
                table: "RouteStations",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetroStations");

            migrationBuilder.DropTable(
                name: "RouteStations");

            migrationBuilder.DropTable(
                name: "BusRoutes");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "Buses");
        }
    }
}
