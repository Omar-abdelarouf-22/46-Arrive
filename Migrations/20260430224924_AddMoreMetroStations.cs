using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Arrive.BusApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreMetroStations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 29.8491, 31.334099999999999 });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.864100000000001, 31.3323, "Ain Helwan" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.8706, 31.328299999999999, "Helwan University" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.882100000000001, 31.3218, "Wadi Hof" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.897200000000002, 31.312899999999999, "Hadayek Helwan" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.907900000000001, 31.303899999999999, "El-Maasara" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.924800000000001, 31.291699999999999, "Tora El-Asmant" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.936599999999999, 31.282900000000001, "Kozzika" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.9482, 31.274999999999999, "Tora El-Balad" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.954599999999999, 31.265699999999999, "Sakanat El-Maadi" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.9602, 31.258500000000002, "Maadi" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.972100000000001, 31.250499999999999, "Hadayek El-Maadi" });

            migrationBuilder.InsertData(
                table: "MetroStations",
                columns: new[] { "Id", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 13, 29.982199999999999, 31.242999999999999, "Dar El-Salam" },
                    { 14, 29.995899999999999, 31.233499999999999, "El-Zahraa" },
                    { 15, 30.0062, 31.23, "Mar Girgis" },
                    { 16, 30.017499999999998, 31.229099999999999, "El-Malek El-Saleh" },
                    { 17, 30.029599999999999, 31.2332, "Al-Sayeda Zeinab" },
                    { 18, 30.035699999999999, 31.236599999999999, "Saad Zaghloul" },
                    { 19, 30.0444, 31.235800000000001, "Sadat" },
                    { 20, 30.0535, 31.238700000000001, "Nasser" },
                    { 21, 30.0579, 31.242699999999999, "Orabi" },
                    { 22, 30.061, 31.245999999999999, "Al-Shohadaa" },
                    { 23, 30.066400000000002, 31.264399999999998, "Ghamra" },
                    { 24, 30.0715, 31.2727, "El-Demerdash" },
                    { 25, 30.075900000000001, 31.276599999999998, "Manshiet El-Sadr" },
                    { 26, 30.081800000000001, 31.284500000000001, "Kobri El-Qobba" },
                    { 27, 30.0854, 31.290500000000002, "Hammamat El-Qobba" },
                    { 28, 30.0898, 31.296500000000002, "Saray El-Qobba" },
                    { 29, 30.098800000000001, 31.306899999999999, "Hadayek El-Zaitoun" },
                    { 30, 30.104399999999998, 31.3125, "Helmeyet El-Zaitoun" },
                    { 31, 30.114699999999999, 31.318100000000001, "El-Matareyya" },
                    { 32, 30.124300000000002, 31.323499999999999, "Ain Shams" },
                    { 33, 30.1387, 31.328299999999999, "Ezbet El-Nakhl" },
                    { 34, 30.152100000000001, 31.3384, "El-Marg" },
                    { 35, 30.1633, 31.336400000000001, "New El-Marg" },
                    { 36, 29.980599999999999, 31.211400000000001, "El-Mounib" },
                    { 37, 29.995799999999999, 31.209800000000001, "Sakiat Mekki" },
                    { 38, 30.005099999999999, 31.2089, "Omm El-Misryeen" },
                    { 39, 30.0137, 31.207899999999999, "Giza" },
                    { 40, 30.021000000000001, 31.204499999999999, "Faisal" },
                    { 41, 30.026, 31.2013, "Cairo University" },
                    { 42, 30.035499999999999, 31.200099999999999, "El-Bohouth" },
                    { 43, 30.038499999999999, 31.2121, "Dokki" },
                    { 44, 30.041899999999998, 31.224900000000002, "Opera" },
                    { 45, 30.0443, 31.244299999999999, "Mohamed Naguib" },
                    { 46, 30.052299999999999, 31.2468, "Attaba" },
                    { 47, 30.070900000000002, 31.245200000000001, "Massara" },
                    { 48, 30.081399999999999, 31.246400000000001, "Rod El-Farag" },
                    { 49, 30.091000000000001, 31.2469, "St. Teresa" },
                    { 50, 30.100899999999999, 31.247499999999999, "Khalafawy" },
                    { 51, 30.106999999999999, 31.249199999999998, "Mezallat" },
                    { 52, 30.113299999999999, 31.2486, "Kolleyyet El-Zeraa" },
                    { 53, 30.122499999999999, 31.244700000000002, "Shubra El-Kheima" },
                    { 54, 30.1465, 31.421700000000001, "Adly Mansour" },
                    { 55, 30.135999999999999, 31.4055, "El Hayestep" },
                    { 56, 30.130199999999999, 31.3855, "Omar Ibn El-Khattab" },
                    { 57, 30.1265, 31.3705, "Qobaa" },
                    { 58, 30.122499999999999, 31.355499999999999, "Hesham Barakat" },
                    { 59, 30.118500000000001, 31.345500000000001, "El-Nozha" },
                    { 60, 30.1145, 31.3355, "Nadi El-Shams" },
                    { 61, 30.109500000000001, 31.325500000000002, "Alf Maskan" },
                    { 62, 30.1005, 31.320499999999999, "Heliopolis Square" },
                    { 63, 30.093499999999999, 31.3155, "Haroun" },
                    { 64, 30.0885, 31.3125, "Al-Ahram" },
                    { 65, 30.083500000000001, 31.3095, "Koleyet El-Banat" },
                    { 66, 30.073499999999999, 31.3065, "Stadium" },
                    { 67, 30.070499999999999, 31.298500000000001, "Fair Zone" },
                    { 68, 30.073499999999999, 31.283100000000001, "Abbassia" },
                    { 69, 30.0655, 31.275500000000001, "Abdo Pasha" },
                    { 70, 30.060500000000001, 31.265499999999999, "El-Geish" },
                    { 71, 30.055499999999999, 31.255500000000001, "Bab El-Shaaria" },
                    { 72, 30.055099999999999, 31.232500000000002, "Maspero" },
                    { 73, 30.0625, 31.2225, "Safaa Hegazy" },
                    { 74, 30.0685, 31.212499999999999, "Kit Kat" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 29.8489, 31.334199999999999 });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 29.9602, 31.2576, "Maadi" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.0444, 31.235800000000001, "Sadat" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.0535, 31.238700000000001, "Nasser" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.061, 31.245999999999999, "Al Shohadaa" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.052299999999999, 31.2468, "Attaba" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.026, 31.2013, "Cairo University" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.038499999999999, 31.2121, "Dokki" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.041899999999998, 31.224900000000002, "Opera" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.073499999999999, 31.283100000000001, "Abbassia" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.101500000000001, 31.333100000000002, "Haroun" });

            migrationBuilder.UpdateData(
                table: "MetroStations",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Latitude", "Longitude", "Name" },
                values: new object[] { 30.152100000000001, 31.3384, "El Marg" });
        }
    }
}
