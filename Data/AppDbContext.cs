using Arrive.BusApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Arrive.BusApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Bus> Buses => Set<Bus>();

    public DbSet<Station> Stations => Set<Station>();

    public DbSet<BusRoute> BusRoutes => Set<BusRoute>();

    public DbSet<RouteStation> RouteStations => Set<RouteStation>();

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
