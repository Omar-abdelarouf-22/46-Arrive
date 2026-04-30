using Arrive.BusApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Arrive.BusApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Bus> Buses => Set<Bus>();

    public DbSet<Station> Stations => Set<Station>();

    public DbSet<BusRoute> BusRoutes => Set<BusRoute>();

    public DbSet<RouteStation> RouteStations => Set<RouteStation>();

    public DbSet<MetroStation> MetroStations => Set<MetroStation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bus>(entity =>
        {
            entity.Property(bus => bus.Number)
                .HasMaxLength(20)
                .IsRequired();
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.Property(station => station.Name)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<BusRoute>(entity =>
        {
            entity.Property(route => route.RouteName)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasOne(route => route.Bus)
                .WithMany(bus => bus.Routes)
                .HasForeignKey(route => route.BusId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RouteStation>(entity =>
        {
            entity.HasOne(routeStation => routeStation.BusRoute)
                .WithMany(route => route.RouteStations)
                .HasForeignKey(routeStation => routeStation.BusRouteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(routeStation => routeStation.Station)
                .WithMany(station => station.RouteStations)
                .HasForeignKey(routeStation => routeStation.StationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(routeStation => new
            {
                routeStation.BusRouteId,
                routeStation.StationOrder
            }).IsUnique();

            entity.HasIndex(routeStation => new
            {
                routeStation.BusRouteId,
                routeStation.StationId
            }).IsUnique();
        });

        modelBuilder.Entity<Bus>().HasData(
            new Bus { Id = 1, Number = "80" },
            new Bus { Id = 2, Number = "90" });

        modelBuilder.Entity<MetroStation>().HasData(
            // Line 1
            new MetroStation { Id = 1, Name = "Helwan", Latitude = 29.8491, Longitude = 31.3341 },
            new MetroStation { Id = 2, Name = "Ain Helwan", Latitude = 29.8641, Longitude = 31.3323 },
            new MetroStation { Id = 3, Name = "Helwan University", Latitude = 29.8706, Longitude = 31.3283 },
            new MetroStation { Id = 4, Name = "Wadi Hof", Latitude = 29.8821, Longitude = 31.3218 },
            new MetroStation { Id = 5, Name = "Hadayek Helwan", Latitude = 29.8972, Longitude = 31.3129 },
            new MetroStation { Id = 6, Name = "El-Maasara", Latitude = 29.9079, Longitude = 31.3039 },
            new MetroStation { Id = 7, Name = "Tora El-Asmant", Latitude = 29.9248, Longitude = 31.2917 },
            new MetroStation { Id = 8, Name = "Kozzika", Latitude = 29.9366, Longitude = 31.2829 },
            new MetroStation { Id = 9, Name = "Tora El-Balad", Latitude = 29.9482, Longitude = 31.2750 },
            new MetroStation { Id = 10, Name = "Sakanat El-Maadi", Latitude = 29.9546, Longitude = 31.2657 },
            new MetroStation { Id = 11, Name = "Maadi", Latitude = 29.9602, Longitude = 31.2585 },
            new MetroStation { Id = 12, Name = "Hadayek El-Maadi", Latitude = 29.9721, Longitude = 31.2505 },
            new MetroStation { Id = 13, Name = "Dar El-Salam", Latitude = 29.9822, Longitude = 31.2430 },
            new MetroStation { Id = 14, Name = "El-Zahraa", Latitude = 29.9959, Longitude = 31.2335 },
            new MetroStation { Id = 15, Name = "Mar Girgis", Latitude = 30.0062, Longitude = 31.2300 },
            new MetroStation { Id = 16, Name = "El-Malek El-Saleh", Latitude = 30.0175, Longitude = 31.2291 },
            new MetroStation { Id = 17, Name = "Al-Sayeda Zeinab", Latitude = 30.0296, Longitude = 31.2332 },
            new MetroStation { Id = 18, Name = "Saad Zaghloul", Latitude = 30.0357, Longitude = 31.2366 },
            new MetroStation { Id = 19, Name = "Sadat", Latitude = 30.0444, Longitude = 31.2358 },
            new MetroStation { Id = 20, Name = "Nasser", Latitude = 30.0535, Longitude = 31.2387 },
            new MetroStation { Id = 21, Name = "Orabi", Latitude = 30.0579, Longitude = 31.2427 },
            new MetroStation { Id = 22, Name = "Al-Shohadaa", Latitude = 30.0610, Longitude = 31.2460 },
            new MetroStation { Id = 23, Name = "Ghamra", Latitude = 30.0664, Longitude = 31.2644 },
            new MetroStation { Id = 24, Name = "El-Demerdash", Latitude = 30.0715, Longitude = 31.2727 },
            new MetroStation { Id = 25, Name = "Manshiet El-Sadr", Latitude = 30.0759, Longitude = 31.2766 },
            new MetroStation { Id = 26, Name = "Kobri El-Qobba", Latitude = 30.0818, Longitude = 31.2845 },
            new MetroStation { Id = 27, Name = "Hammamat El-Qobba", Latitude = 30.0854, Longitude = 31.2905 },
            new MetroStation { Id = 28, Name = "Saray El-Qobba", Latitude = 30.0898, Longitude = 31.2965 },
            new MetroStation { Id = 29, Name = "Hadayek El-Zaitoun", Latitude = 30.0988, Longitude = 31.3069 },
            new MetroStation { Id = 30, Name = "Helmeyet El-Zaitoun", Latitude = 30.1044, Longitude = 31.3125 },
            new MetroStation { Id = 31, Name = "El-Matareyya", Latitude = 30.1147, Longitude = 31.3181 },
            new MetroStation { Id = 32, Name = "Ain Shams", Latitude = 30.1243, Longitude = 31.3235 },
            new MetroStation { Id = 33, Name = "Ezbet El-Nakhl", Latitude = 30.1387, Longitude = 31.3283 },
            new MetroStation { Id = 34, Name = "El-Marg", Latitude = 30.1521, Longitude = 31.3384 },
            new MetroStation { Id = 35, Name = "New El-Marg", Latitude = 30.1633, Longitude = 31.3364 },
            
            // Line 2
            new MetroStation { Id = 36, Name = "El-Mounib", Latitude = 29.9806, Longitude = 31.2114 },
            new MetroStation { Id = 37, Name = "Sakiat Mekki", Latitude = 29.9958, Longitude = 31.2098 },
            new MetroStation { Id = 38, Name = "Omm El-Misryeen", Latitude = 30.0051, Longitude = 31.2089 },
            new MetroStation { Id = 39, Name = "Giza", Latitude = 30.0137, Longitude = 31.2079 },
            new MetroStation { Id = 40, Name = "Faisal", Latitude = 30.0210, Longitude = 31.2045 },
            new MetroStation { Id = 41, Name = "Cairo University", Latitude = 30.0260, Longitude = 31.2013 },
            new MetroStation { Id = 42, Name = "El-Bohouth", Latitude = 30.0355, Longitude = 31.2001 },
            new MetroStation { Id = 43, Name = "Dokki", Latitude = 30.0385, Longitude = 31.2121 },
            new MetroStation { Id = 44, Name = "Opera", Latitude = 30.0419, Longitude = 31.2249 },
            new MetroStation { Id = 45, Name = "Mohamed Naguib", Latitude = 30.0443, Longitude = 31.2443 },
            new MetroStation { Id = 46, Name = "Attaba", Latitude = 30.0523, Longitude = 31.2468 },
            new MetroStation { Id = 47, Name = "Massara", Latitude = 30.0709, Longitude = 31.2452 },
            new MetroStation { Id = 48, Name = "Rod El-Farag", Latitude = 30.0814, Longitude = 31.2464 },
            new MetroStation { Id = 49, Name = "St. Teresa", Latitude = 30.0910, Longitude = 31.2469 },
            new MetroStation { Id = 50, Name = "Khalafawy", Latitude = 30.1009, Longitude = 31.2475 },
            new MetroStation { Id = 51, Name = "Mezallat", Latitude = 30.1070, Longitude = 31.2492 },
            new MetroStation { Id = 52, Name = "Kolleyyet El-Zeraa", Latitude = 30.1133, Longitude = 31.2486 },
            new MetroStation { Id = 53, Name = "Shubra El-Kheima", Latitude = 30.1225, Longitude = 31.2447 },

            // Line 3
            new MetroStation { Id = 54, Name = "Adly Mansour", Latitude = 30.1465, Longitude = 31.4217 },
            new MetroStation { Id = 55, Name = "El Hayestep", Latitude = 30.1360, Longitude = 31.4055 },
            new MetroStation { Id = 56, Name = "Omar Ibn El-Khattab", Latitude = 30.1302, Longitude = 31.3855 },
            new MetroStation { Id = 57, Name = "Qobaa", Latitude = 30.1265, Longitude = 31.3705 },
            new MetroStation { Id = 58, Name = "Hesham Barakat", Latitude = 30.1225, Longitude = 31.3555 },
            new MetroStation { Id = 59, Name = "El-Nozha", Latitude = 30.1185, Longitude = 31.3455 },
            new MetroStation { Id = 60, Name = "Nadi El-Shams", Latitude = 30.1145, Longitude = 31.3355 },
            new MetroStation { Id = 61, Name = "Alf Maskan", Latitude = 30.1095, Longitude = 31.3255 },
            new MetroStation { Id = 62, Name = "Heliopolis Square", Latitude = 30.1005, Longitude = 31.3205 },
            new MetroStation { Id = 63, Name = "Haroun", Latitude = 30.0935, Longitude = 31.3155 },
            new MetroStation { Id = 64, Name = "Al-Ahram", Latitude = 30.0885, Longitude = 31.3125 },
            new MetroStation { Id = 65, Name = "Koleyet El-Banat", Latitude = 30.0835, Longitude = 31.3095 },
            new MetroStation { Id = 66, Name = "Stadium", Latitude = 30.0735, Longitude = 31.3065 },
            new MetroStation { Id = 67, Name = "Fair Zone", Latitude = 30.0705, Longitude = 31.2985 },
            new MetroStation { Id = 68, Name = "Abbassia", Latitude = 30.0735, Longitude = 31.2831 },
            new MetroStation { Id = 69, Name = "Abdo Pasha", Latitude = 30.0655, Longitude = 31.2755 },
            new MetroStation { Id = 70, Name = "El-Geish", Latitude = 30.0605, Longitude = 31.2655 },
            new MetroStation { Id = 71, Name = "Bab El-Shaaria", Latitude = 30.0555, Longitude = 31.2555 },
            new MetroStation { Id = 72, Name = "Maspero", Latitude = 30.0551, Longitude = 31.2325 },
            new MetroStation { Id = 73, Name = "Safaa Hegazy", Latitude = 30.0625, Longitude = 31.2225 },
            new MetroStation { Id = 74, Name = "Kit Kat", Latitude = 30.0685, Longitude = 31.2125 }
        );

        modelBuilder.Entity<Station>().HasData(
            new Station { Id = 1, Name = "مساكن اسكو", Latitude = 30.1369, Longitude = 31.3217 },
            new Station { Id = 2, Name = "بهتيم", Latitude = 30.1407, Longitude = 31.2968 },
            new Station { Id = 3, Name = "الشارع الجديد", Latitude = 30.1298, Longitude = 31.3095 },
            new Station { Id = 4, Name = "مسطرد", Latitude = 30.1389, Longitude = 31.2935 },
            new Station { Id = 5, Name = "المطرية", Latitude = 30.1171, Longitude = 31.3131 },
            new Station { Id = 6, Name = "الحلمية", Latitude = 30.0957, Longitude = 31.2876 },
            new Station { Id = 7, Name = "التجنيد", Latitude = 30.0867, Longitude = 31.3075 },
            new Station { Id = 8, Name = "محكمة", Latitude = 30.0841, Longitude = 31.3032 },
            new Station { Id = 9, Name = "روكسي", Latitude = 30.0911, Longitude = 31.3197 },
            new Station { Id = 10, Name = "ارض الجولف", Latitude = 30.0891, Longitude = 31.3421 },
            new Station { Id = 11, Name = "الخليفة المأمون", Latitude = 30.0797, Longitude = 31.2982 },
            new Station { Id = 12, Name = "كوبري القبة", Latitude = 30.0871, Longitude = 31.2946 },
            new Station { Id = 13, Name = "حدائق القبة", Latitude = 30.0864, Longitude = 31.2841 },
            new Station { Id = 14, Name = "حمامات القبة", Latitude = 30.0819, Longitude = 31.2782 },
            new Station { Id = 15, Name = "منشية الصدر", Latitude = 30.0754, Longitude = 31.2749 },
            new Station { Id = 16, Name = "مستشفى الدمرداش", Latitude = 30.0707, Longitude = 31.2717 },
            new Station { Id = 17, Name = "عرب", Latitude = 30.1327, Longitude = 31.3009 },
            new Station { Id = 18, Name = "مستشفى الجلاء", Latitude = 30.0614, Longitude = 31.2458 },
            new Station { Id = 19, Name = "شارع رمسيس", Latitude = 30.0619, Longitude = 31.2465 },
            new Station { Id = 20, Name = "ميدان الجيزة", Latitude = 30.0106, Longitude = 31.2086 });

        modelBuilder.Entity<BusRoute>().HasData(
            new BusRoute { Id = 1, BusId = 1, RouteName = "خط 80 - مساكن اسكو إلى مستشفى الدمرداش" },
            new BusRoute { Id = 2, BusId = 2, RouteName = "خط 90 - مساكن اسكو إلى ميدان الجيزة" });

        modelBuilder.Entity<RouteStation>().HasData(
            new RouteStation { Id = 1, BusRouteId = 1, StationId = 1, StationOrder = 1 },
            new RouteStation { Id = 2, BusRouteId = 1, StationId = 2, StationOrder = 2 },
            new RouteStation { Id = 3, BusRouteId = 1, StationId = 3, StationOrder = 3 },
            new RouteStation { Id = 4, BusRouteId = 1, StationId = 4, StationOrder = 4 },
            new RouteStation { Id = 5, BusRouteId = 1, StationId = 5, StationOrder = 5 },
            new RouteStation { Id = 6, BusRouteId = 1, StationId = 6, StationOrder = 6 },
            new RouteStation { Id = 7, BusRouteId = 1, StationId = 7, StationOrder = 7 },
            new RouteStation { Id = 8, BusRouteId = 1, StationId = 8, StationOrder = 8 },
            new RouteStation { Id = 9, BusRouteId = 1, StationId = 9, StationOrder = 9 },
            new RouteStation { Id = 10, BusRouteId = 1, StationId = 10, StationOrder = 10 },
            new RouteStation { Id = 11, BusRouteId = 1, StationId = 11, StationOrder = 11 },
            new RouteStation { Id = 12, BusRouteId = 1, StationId = 12, StationOrder = 12 },
            new RouteStation { Id = 13, BusRouteId = 1, StationId = 13, StationOrder = 13 },
            new RouteStation { Id = 14, BusRouteId = 1, StationId = 14, StationOrder = 14 },
            new RouteStation { Id = 15, BusRouteId = 1, StationId = 15, StationOrder = 15 },
            new RouteStation { Id = 16, BusRouteId = 1, StationId = 16, StationOrder = 16 },
            new RouteStation { Id = 17, BusRouteId = 2, StationId = 1, StationOrder = 1 },
            new RouteStation { Id = 18, BusRouteId = 2, StationId = 2, StationOrder = 2 },
            new RouteStation { Id = 19, BusRouteId = 2, StationId = 17, StationOrder = 3 },
            new RouteStation { Id = 20, BusRouteId = 2, StationId = 4, StationOrder = 4 },
            new RouteStation { Id = 21, BusRouteId = 2, StationId = 5, StationOrder = 5 },
            new RouteStation { Id = 22, BusRouteId = 2, StationId = 9, StationOrder = 6 },
            new RouteStation { Id = 23, BusRouteId = 2, StationId = 18, StationOrder = 7 },
            new RouteStation { Id = 24, BusRouteId = 2, StationId = 19, StationOrder = 8 },
            new RouteStation { Id = 25, BusRouteId = 2, StationId = 20, StationOrder = 9 });
    }
}
