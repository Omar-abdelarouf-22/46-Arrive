using Arrive.BusApi.Data;
using Arrive.BusApi.Dtos;
using Arrive.BusApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;

namespace Arrive.BusApi.Controllers;

[ApiController]
[Route("api/bus/routes")]
public class BusRoutesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("stations")]
    public async Task<ActionResult<IReadOnlyList<StationDto>>> GetStations()
    {
        var stations = await dbContext.Stations
            .AsNoTracking()
            .OrderBy(station => station.Name)
            .Select(station => new StationDto(
                station.Id,
                station.Name,
                station.Latitude,
                station.Longitude,
                0))
            .ToListAsync();

        return Ok(stations);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusRouteDto>>> GetRoutes()
    {
        var routes = await dbContext.BusRoutes
            .AsNoTracking()
            .Include(route => route.Bus)
            .Include(route => route.RouteStations)
                .ThenInclude(routeStation => routeStation.Station)
            .OrderBy(route => route.Id)
            .ToListAsync();

        return Ok(routes.Select(ToRouteDto).ToList());
    }

    [HttpPost("search")]
    public async Task<ActionResult<IReadOnlyList<BusRouteSearchResult>>> SearchRoute(BusRouteSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.From) || string.IsNullOrWhiteSpace(request.To))
        {
            return BadRequest("Please enter both FROM and TO stations.");
        }

        var fromText = Normalize(request.From);
        var toText = Normalize(request.To);

        var routes = await dbContext.BusRoutes
            .AsNoTracking()
            .Include(route => route.Bus)
            .Include(route => route.RouteStations)
                .ThenInclude(routeStation => routeStation.Station)
            .ToListAsync();

        var results = new List<BusRouteSearchResult>();

        foreach (var route in routes)
        {
            var orderedStations = route.RouteStations
                .OrderBy(routeStation => routeStation.StationOrder)
                .ToList();

            var fromStation = FindBestStationMatch(orderedStations, fromText);
            var toStation = FindBestStationMatch(orderedStations, toText);

            if (fromStation is null || toStation is null)
            {
                continue;
            }

            if (fromStation.StationOrder == toStation.StationOrder)
            {
                return BadRequest("FROM and TO cannot be the same station.");
            }

            var tripStations = GetTripStations(orderedStations, fromStation, toStation);

            results.Add(new BusRouteSearchResult(
                route.Id,
                route.RouteName,
                route.Bus?.Number ?? string.Empty,
                fromStation.Station!.Name,
                toStation.Station!.Name,
                tripStations.Select(ToStationDto).ToList()));
        }

        if (results.Count > 0)
        {
            return Ok(results);
        }

        return NotFound("No bus route was found for these stations.");
    }

    private static Models.RouteStation? FindBestStationMatch(
        IReadOnlyList<Models.RouteStation> orderedStations,
        string normalizedQuery)
    {
        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return null;
        }

        return orderedStations.FirstOrDefault(routeStation =>
                routeStation.Station is not null &&
                Normalize(routeStation.Station.Name) == normalizedQuery)
            ?? orderedStations.FirstOrDefault(routeStation =>
                routeStation.Station is not null &&
                Normalize(routeStation.Station.Name).Contains(normalizedQuery))
            ?? orderedStations.FirstOrDefault(routeStation =>
                routeStation.Station is not null &&
                normalizedQuery.Contains(Normalize(routeStation.Station.Name)));
    }

    private static BusRouteDto ToRouteDto(Models.BusRoute route)
    {
        return new BusRouteDto(
            route.Id,
            route.RouteName,
            route.Bus?.Number ?? string.Empty,
            route.RouteStations
                .OrderBy(routeStation => routeStation.StationOrder)
                .Select(ToStationDto)
                .ToList());
    }

    private static StationDto ToStationDto(Models.RouteStation routeStation)
    {
        var station = routeStation.Station!;

        return new StationDto(
            station.Id,
            station.Name,
            station.Latitude,
            station.Longitude,
            routeStation.StationOrder);
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        var normalized = value.Trim().ToLowerInvariant()
            .Replace('\u0623', '\u0627')
            .Replace('\u0625', '\u0627')
            .Replace('\u0622', '\u0627')
            .Replace('\u0629', '\u0647')
            .Replace('\u0649', '\u064A')
            .Replace('\u0626', '\u064A')
            .Replace('\u0624', '\u0648');

        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"(^|\s)\u0627\u0644", "$1");

        return normalized;
    }

    private static List<Models.RouteStation> GetTripStations(
        IReadOnlyList<Models.RouteStation> orderedStations,
        Models.RouteStation fromStation,
        Models.RouteStation toStation)
    {
        var isForwardTrip = fromStation.StationOrder < toStation.StationOrder;
        var startOrder = Math.Min(fromStation.StationOrder, toStation.StationOrder);
        var endOrder = Math.Max(fromStation.StationOrder, toStation.StationOrder);

        var tripStations = orderedStations
            .Where(routeStation =>
                routeStation.StationOrder >= startOrder &&
                routeStation.StationOrder <= endOrder)
            .ToList();

        if (!isForwardTrip)
        {
            tripStations.Reverse();
        }

        return tripStations;
    }

    [HttpPost("sync-coordinates")]
    public async Task<IActionResult> SyncCoordinates()
    {
        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "station_coordinates.csv");
        if (!System.IO.File.Exists(csvPath))
        {
            return NotFound("station_coordinates.csv was not found.");
        }

        var stationCoords = await LoadStationCoordinates(csvPath);
        if (stationCoords.Count == 0)
        {
            return BadRequest("No valid station coordinates were found in station_coordinates.csv.");
        }

        var stations = await dbContext.Stations.ToListAsync();
        var updated = 0;

        foreach (var station in stations)
        {
            if (!stationCoords.TryGetValue(station.Name, out var coords))
            {
                continue;
            }

            station.Latitude = coords.lat;
            station.Longitude = coords.lon;
            updated++;
        }

        await dbContext.SaveChangesAsync();
        return Ok(new { updated, total = stations.Count });
    }

    private static async Task<Dictionary<string, (double lat, double lon)>> LoadStationCoordinates(string csvPath)
    {
        var stationCoords = new Dictionary<string, (double lat, double lon)>();
        var csvLines = await System.IO.File.ReadAllLinesAsync(csvPath);

        foreach (var csvLine in csvLines.Skip(1))
        {
            var columns = ParseCsvLine(csvLine);
            if (columns.Count < 4)
            {
                continue;
            }

            var stopName = columns[0].Trim();
            if (double.TryParse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(columns[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var lon))
            {
                stationCoords[stopName] = (lat, lon);
            }
        }

        return stationCoords;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (ch == ',' && !inQuotes)
            {
                values.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(ch);
            }
        }

        values.Add(current.ToString());
        return values;
    }

    [HttpPost("seed-new-data")]
    public async Task<IActionResult> SeedNewData()
    {
        var rawData = @"82: احمد حلمى - قللى - رمسيس - غمرة - دمرداش - عباسية - صلاح سالم - شارع الطيران - خضر التونى - رابعة العدوية - اول عباس - سيتى ستارز - مكرم عبيد - تقاطع م النحاس - السراج مول - الوفاء والامل - جامع السلام - سوق السيارات - بوابات - الحى العاشر - زهراء مدينة نصر - التجمع الاول
80: مساكن أسكو - بهتيم - شارع الجديد - مسطرد - المطرية - الحلمية - التجنيد - محكمة - روكسى - الخليفة المأمون - كوبرى القبة - جامعة عين شمس - عباسية - ميدان الجيش - م عبده باشا - م سيد جلال - باب الشعرية - البهناوى - مستشفى الحسين - الدراسة
259: مساكن أسكو - بهتيم - شارع الجديد - مسطرد - المطرية - الحلمية - التجنيد - المحكمة - سانت فاتيما - سفير - تريامف - ميدان الحجاز - مستشفى العيون - الكلية الحربية - مساكن شيراتون
87: مساكن أسكو - بهتيم - الشارع الجديد - مسطرد - كوبرى السواح - اميرية - حدائق القبة - كوبرى الفنجري - نادي السكة - جامعة الأزهر - رابعة العدوية - التأمين الصحي - نوري خطاب - الحي السابع - إنبي - الوفاء والامل - جامع السلام - بوابات - الحي العاشر - زهراء مدينة نصر - أكاديمية الشرطة - التجمع الاول
88: مساكن أسكو - بهتيم - الشارع الجديد - كوبرى عرابي - مظلات - شارع شبرا - الخازندارة - دوران شبرا - مستشفى الرمد - روض الفرج - وكالة البلح - الإذاعة والتليفزيون - تحرير - كورنيش - باب اللوق - احمد ماهر - شارع قدري - القلعة - السيدة عائشة
28: بيجام (ابن سينا) - مؤسسة - كوبرى عرابي - مظلات - خلفاوى - الزاوية الحمراء - شارع سكة الوالي - حدائق القبة - كوبرى القبة - الخليفة المأمون - منشية البكري - نادي هليوبوليس - روكسى - كورية - الحرية مول - م كيلوباترا - م صلاح الدين - ميدان الإسماعيلية - م سفير - م سانت فاتيما - ميدان الحجاز - مستشفى العيون - مدرسة السيدة خديجة - الكلية الحربية - مساكن شيراتون
25: مساكن الشروق - النادي الأهلي - احمد فخري - معهد الخدمة - اول مكرم - اول عباس - رابعة العدوية - جامعة الأزهر - نادي السكة - وزارة المالية - عباسية - دمرداش - مترو غمرة - رمسيس - إسعاف - التحرير
84: احمد حلمى - قللى - رمسيس - غمرة - دمرداش - عباسية - صلاح سالم - المعرض - شارع الطيران - رابعة - التأمين الصحي - نوري خطاب - الحي السابع - إنبي - الوفاء والامل - جامع السلام - سوق السيارات - البوابات - الحي العاشر - التبة
26: بيجام - مؤسسة - شبرا الخيمة - الشارع الجديد - مسطرد - مطرية - حلمية الزيتون - تجنيد - محكمة - سفير - نادي الجلاء - السبع عمارات - شارع النزهة - الرقابة الإدارية - اول عباس - ميدان الساعة - اول مكرم - معهد الخدمة الاجتماعية - احمد فخري - النادي الأهلي - مساكن الشروق
115: مساكن الوحدة العربية - أبو الهنا - شارع الجديد - ك مسطرد - الرشاح - التعاون - المطرية - الحلمية - التجنيد - المحكمة - هارون - سفير - السبع عمارات - الميرغني - كلية البنات - عمارات المروة - خضر التوني - شارع الطيران - فندق سونستا - وزارة الدفاع - رابعة - التأمين الصحي - نوري خطاب - الحي السابع - إنبي - الوفاء و الامل - جامع السلام - سوق السيارات - بوابات - الحي العاشر - التبة
89: مساكن أسكو - بهتيم - الشارع الجديد - مسطرد - سواح - الزاوية الحمراء - الوالي - غمرة - باب الشعرية - موسكى - باب الخلق - احمد ماهر - حدائق زينهم - مستشفى المقطم - السيدة زينب - ابو الريش - محكمة زينهم - السيدة نفيسة
90: مساكن اسكو - المزرعة - الشارع الجديد - كوبرى عرابي - مظلات - شارع شبرا - دوران شبرا - مسرة - احمد حلمى - قللى - رمسيس - جامع الفتح - شارع الجمهورية - عتبة - جامع البنات - باب الخلق - احمد ماهر - شارع قدري - السيدة زينب - عمارات العرائس - القصر العيني - طب الاسنان - المنيل - حديقة الحيوان - جامعة القاهرة - ميدان الجيزة
162: مؤسسة - كوبرى عرابي - الشارع الجديد - مسطرد - التعاون - ميدان المطرية - حلمية الزيتون - محكمة - هارون - سفير - السبع عمارات - كوبرى الجلاء - مستشفى فلسطين - شارع الثورة - ألماظة - دار الإشارة - كوبرى ألماظة - سيتي سنتر ألماظة - مستشفى الكهرباء - عزبة الهجانة - كيلو 4.5 - مدينة الأمل (عزبة الهجانة)
72: مظلات - عبود - معدية عثمان - سواح - اميرية - سراى القبة - روكسى - كورية - نفق الثورة - سنترال ألماظة - السبع عمارات - شارع النزهة - الرقابة الإدارية - اول عباس - ميدان الساعة - اول مكرم - احمد فخري - خدمة اجتماعية - نادي الأهلي - مساكن الشروق
18: مساكن أسكو - بهتيم - الشارع الجديد - مسطرد - الأميرية - سواح - سرايا القبة - روكسى - كورية - نفق الثورة - الميرغني - كلية البنات - خضر التوني - رابعة العدوية - اول عباس - اول مكرم - أحمد فخري - سيتي ستارز - النادي الأهلي - مساكن الشروق
203: العمرانية - ميدان الجيزة - جامعة القاهرة - القصر العيني - شارع القصر العيني - تحرير - ميدان عبد المنعم رياض - إسعاف - رمسيس - غمرة - عباسية - امتداد رمسيس - نادي السكة - جامعة الأزهر - رابعة - اول عباس - اول مكرم - النادي الأهلي - عزبة الهجانة الكيلو 4.5
107: مساكن أسكو - المزرعة - الشارع الجديد - كوبرى عرابي - مظلات - خلفاوى - دوران شبرا - شارع شبرا - مسرة - احمد حلمى - قللى - رمسيس - شارع الجلاء - القصر العيني - معهد الاورام - الملك صالح - فم الخليج - مستشفى السلام الدولي - مستشفى المعادي - المحكمة الدستورية - المطبعة
380: العمرانية - ميدان الجيزة - الرمد - المنيل - الملك صالح - فم الخليج - معهد الاورام - مجرى العيون - مستشفى المقطم - عين الصيرة - السيدة عائشة - المنشية - مزلقان الدويقة - المقاولون العرب - نادي السكة - جامعة الأزهر - رابعة - التأمين الصحي - نوري خطاب - مصطفى النحاس - الحي الثامن - المنهل - التبة
56: بيتشو - زهراء المعادي - نفق الزهراء - العرب شارع 77 - كوبرى المعادي - ميدان الحرية - مصر حلوان الزراعي - المطبعة - مستشفى المعادي - المحكمة الدستورية - كورنيش - اندريا - زهراء مصر القديمة - الملك صالح - فم الخليج - القصر العيني - تحرير - إسعاف - رمسيس - غمرة - دمرداش - عباسية - وزارة المالية - نادي السكة - جامعة الأزهر - رابعة - هشام بركات - طريق النصر - طيبة مول - ميدان الساعة - عباس العقاد - مصطفى النحاس - الحي الثامن - المنهل - التبة
12: ميدان الجيزة - شارع فيصل - شارع العشرين - الطالبية - الطوابق - المريوطية - اخر فيصل - ميدان الرماية - مساكن - تجنيد - هضبة الهرم
200: ميدان الحجاز - هليوبلس - محكمة - روكسى - جامعة عين شمس - عباسية - غمرة - رمسيس - إسعاف - تحرير - دقى - جامعة القاهرة - ميدان الجيزة - الطالبية - المريوطية - ميدان الرماية - هضبة الهرم
304: المظلات - خلفاوى - كوبرى أبو وافية - الزاوية الحمراء - حدائق القبة - كوبرى القبة - دار المشاة - كوبرى الفنجري - نادي السكة - جامعة الأزهر - رابعة - طيبه مول - عباس العقاد - ويندر لاند - إنبي - الوفاء و الامل - جامع السلام - سوق السيارات - بوابات - الحى العاشر - زهراء مدينة نصر - أكاديمية الشرطة - التجمع الاول
G58: العمرانية - الكونيسة - أم المصريين - ميدان الجيزة - كوبرى الجامعة - جامعة القاهرة - حديقة الحيوان - القصر العيني - شارع القصر العيني - تحرير - ميدان عبد المنعم رياض - إسعاف - رمسيس - غمرة - دمرداش - عباسية - امتداد رمسيس - وزارة المالية - نادي السكة - جامعة الأزهر - رابعة - هشام بركات - طريق النصر - طيبة مول - اول عباس - اول مكرم - سيتي ستارز - احمد فخري - النادي الأهلي - مدينة الأمل - الكيلو 4.5
333: هضبة الهرم - ميدان الرماية - شارع الهرم - ميدان الجيزة - جامعة القاهرة - القصر العيني - ابو الريش - معهد الاورام - معهد السكر - كوبرى الطبيبي - فم الخليج - السيدة زينب - الحوض المرصوص - احمد ماهر - جامع البنات - شارع الأزهر - عتبة
79: عرب الطوالية - المسلة الجديدة - مسطرد - سواح - الوالي - الزاوية الحمراء - القصيرين - سوق غمرة - الشادر - غمرة - سيد جلال - باب الشعرية - موسكى - جامع البنات - باب الخلق - احمد ماهر - شارع قدري - القلعة - السيدة عائشة - البساتين
10: ابراهيم بك - شارع الجديد - مسطرد - سواح - الاميرية - سراى القبة - روكسى - الكورية - الميرغني - شارع الثورة - سنترال ألماظة - كلية البنات - خضر التوني - رابعة - عباس العقاد - مصطفى النحاس - الحي الثامن - المنهل - التبة - زهراء مدينة نصر - التجمع الاول
129: عرب الطوالية - المسلة - مطرية - سواح - مجمع الاميرية - الكابلات - حدائق القبة - مصر والسودان - احمد سعيد - ميدان الجيش - باب الشعرية - عتبة
78: العمرانية - ميدان الجيزة - جامعة القاهرة - القصر العيني - السيدة زينب - جامع البنات - احمد ماهر - الموسكى - باب الشعرية - البهناوى - الدراسة
M947: كلية الهندسة - دوران شبرا - خلفاوى - مظلات - عبود - تدريب - ترعة الإسماعيلية - معدية عثمان - أدوية - اميرية - سواح - سراى القبة - روكسى - شارع الاهرام - كورية - نفق الثورة - سنترال ألماظة - السبع عمارات - شارع النزهة - الرقابة الإدارية - ميدان الساعة - عباس العقاد - مصطفى النحاس - الحي الثامن - المنهل - التبة - الحي العاشر - زهراء مدينة نصر - التجمع الاول - صنية الشباب - الرحاب - بوابة 9.6.23 - دار مصر
202: عرب الطوالية - المسلة الجديدة - مطرية - منشية الصدر - مجمع الاميرية - حدائق القبة - كوبرى القبة - جامعة عين شمس - عباسية - عبده باشا - كلية الهندسة - ميدان الجيش - مستشفى الحسين - ميدان الحلبى - الدراسة
775: شبرا الخيمة - مؤسسة - كوبرى عرابي - ابو الهنا - الشارع الجديد - مسطرد - الرشاح - التعاون - مطرية - حلمية - محكمة - سفير - كوبرى الجلاء - السبع عمارات - شارع النزهة - الرقابة الإدارية - ميدان الساعة - عباس العقاد - الحديقة الدولية - مصطفى النحاس - الحي الثامن - المنهل - التبة - الحي العاشر - زهراء مدينة نصر - التجمع الاول - صنية الشباب - الرحاب - بوابة 6.9.23.24 - النائب العام - دار مصر
29: موقف مسطرد الجديد - المسلة الجديدة - الكابلات - الرشاح - مطرية - سواح - ميدان السعودية - حدائق القبة - مترو - مصر والسودان - احمد سعيد - ميدان الجيش - الظاهر - باب الشعرية - موسكى - جامع البنات - باب الخلق - احمد ماهر - السيدة زينب - القصر العيني - حديقة الحيوان - جامعة القاهرة - ميدان الجيزة
Q9: الزاوية الحمراء - سكة الوالي - كوبرى القبة - المقريزي - ابن سندر - الخليفة المأمون - إشارة روكسى - نادي هليوبلس - شارع الحرية - بيروت - صلاح الدين - ميدان الإسماعيلية - سفير - تريومف - سانت فاتيما - ميدان الحجاز - الف مسكن - نادي الشمس - جسر السويس - عرب - جراج - حديقة بدر - بزتى - سوق القنال - قباء - اول جمال - موقف العاشر - سوق العبور - كارفور العبور - صينية الخامس - المدينة - الجامع الكبير - عبور شباب - العبور
117: مساكن اسكو - بهتيم - الشارع الجديد - قهوة شرف - كوبرى عرابي - كلية الزراعة - مظلات - خلفاوى - دوران شبرا - مستشفى الرمد - روض الفرج - كورنيش - ممشى اهل مصر - الوكالة - الإذاعة والتليفزيون - تحرير - القصر العيني - طب اسنان - كوبرى الجامعة - حديقة الحيوان - جامعة القاهرة - ميدان الجيزة - أم المصريين - عمرانية
Q8: الزاوية الحمراء - سكة الوالي - حدائق القبة - كوبرى القبة - كوبرى الفنجري - نادي السكة - جامعة الأزهر - رابعة العدوية - هشام بركات - طريق النصر - طيبة مول - ميدان الساعة - عباس العقاد - مصطفى النحاس - الحي الثامن - المنهل - المعصراوى - التبة - الحي العاشر - زهراء مدينة نصر - أكاديمية الشرطة - الدائري - كايرو فيستفال - شارع التسعين - الغاز - المستشفى الجوي - جامعة المستقبل - الجامعة الأمريكية - التجمع الخامس
Q45: المطبعة - مستشفى المعادي - كوبرى العاشر - مساكن الفسطاط - الخيالة - نادي الأبطال - السيدة عائشة - المنشية - الدويقة - المقاولون - نادي السكة - جامعة الأزهر - رابعة - التأمين الصحي - مسجد نوري خطاب - مصطفى النحاس - الحي الثامن - المنهل - جامع السلام - بوابات - الحي العاشر - زهراء مدينة نصر - التجمع الاول
Q315: مساكن عين شمس - جراج - عرب - الف مسكن - تجنيد - حلمية - ابن الحكم - مطرية - اميرية - سواح - عبود - مظلات - خلفاوى - شارع شبرا - روض الفرج - كورنيش النيل - الوكالة - كوبرى 15 مايو - زمالك - احمد عرابي - بشتيل
Q216: صقر قريش - الأوتوستراد - الاباجية - السيدة عائشة - السبياني - باب الخلق - حسن الأكبر - باب اللوق - الفلكي - القصر العيني - جامعة القاهرة - الدقى - شارع البطل أحمد عبد العزيز - جامعة الدول العربية - احمد عرابي - بشتيل
Q380: العمرانية - ميدان الجيزة - الرمد - المنيل - الملك صالح - فم الخليج - معهد الاورام - مجرى العيون - مستشفى المقطم - السيدة عائشة - المنشية - الدويقة - المقاولون - نادي السكة - جامعة الأزهر - رابعة - التأمين الصحي - مسجد نوري خطاب - مصطفى النحاس - الحي الثامن - المنهل - التبة - الحي العاشر - زهراء مدينة نصر
Q80: أم بيومي - المنشية - البنزيمة - كوبرى عرابي - كلية الزراعة - مظلات - خلفاوى - شارع شبرا - قسم الساحل - روض الفرج - مسرة - أول شبرا - قللي - رمسيس - مستشفى القبطي - غمرة - دمرداش - عباسية - عبده باشا - ميدان الجيش - الظاهر - مستشفى سيد جلال - باب الشعرية - المعز - دراسة
580: بيجام - قهوة شرف - الشارع الجديد - مسطرد - التعاون - المطرية - حلمية الزيتون - تجنيد - محكمة - هارون - سفير - صلاح الدين - كورية - روكسى - الخليفة المأمون - كوبرى القبة - جامعة عين شمس - عباسية - عبده باشا - ميدان الجيش - باب الشعرية - موسكى - عتبة
144: المطبعة - المحكمة الدستورية - مستشفى القوات المسلحة - المحكمة الدستورية - كورنيش النيل - فم الخليج - معهد الاورام - القصر العيني - كوبرى الطبيبي - السيدة زينب - احمد ماهر - موسكى - باب الشعرية - ميدان الجيش - عبده باشا - عباسية - وزارة المالية - نادي السكة - جامعة الأزهر - رابعة العدوية - التأمين الصحي - مسجد نوري خطاب - مصطفى النحاس - الحي الثامن - المنهل - المعصراوى - التبة - الحي العاشر - زهراء مدينة نصر
Q115: مؤسسة - شبرا الخيمة - كوبرى عرابي - الشارع الجديد - مسطرد - التعاون - المطرية - حلمية الزيتون - محكمة مصر الجديدة - تجنيد - السبع عمارات - الرقابة الإدارية - شارع النزهة - ميدان الساعة - عباس العقاد - البطراوي - مصطفى النحاس - الحي الثامن - المنهل - المعصراوى - التبة - مستشفى الماسة - الحي العاشر - شارع الميثاق - زهراء مدينة نصر - سنترال - أكاديمية الشرطة - هايبر لولو - صنية الشباب - موقف التجمع الأول
Q10: سنديون - مدخل طنان - قلما - كفر أبو جمعة - قليوب - ميت حلفا - الدائري (الأتوبيس الترددي) - ميت نما - مترو المؤسسة
Q5: قليوب المحطة - الاجينة - السعادنة - كوم اشفين - عرب الكوري - بلقس - الدائري (الأتوبيس الترددي) - مساكن أسكو
20: الحوامدية - طريق الصعيد الزراعي - المنيب - ام المصريين - ميدان الجيزة - جامعة القاهرة
Q22: جامعة القاهرة - باب تجارة - حديقة الحيوان - ميدان الجيزة - شارع فيصل - العشرين - المطبعة - المريوطية - الرماية - بوابات 1,2,3,4 - طريق الواحات - دريم بارك - دريم لاند - سوق الجملة - الإنتاج الإعلامي - مول مصر - المتميز - جامعة نوال - الحصري - الحي السادس - ليلة القدر - 6 اكتوبر
500: موقف حدائق القبة الحضاري - جامعة عين شمس - عباسية - مستشفى الزهراء - عبده باشا - ميدان الجيش - الظاهر - سيد جلال - شارع بورسعيد - باب الشعرية - موسكى - جامع البنات - احمد ماهر - السيدة زينب - مستشفى 57357 - مجرى العيون - كورنيش النيل - الملك صالح - مستشفى السلام - المطبعة
Q10: أم بيومي - كوبرى عرابي - قهوة شرف - الملف - الشارع الجديد - مسطرد - المطرية - سواح - أميرية - سراي القبة - روكسى - الخليفة المأمون - كورية - شارع الميرغني - نفق الثورة - سنترال ألماظة - كلية البنات - خضر التوني - رابعة العدوية - شارع الطيران - مسجد نور الخطاب - التأمين الصحي - الحي السابع
Q107: دائري مساكن اسكو - الشارع الجديد - مسطرد - سواح - الزاوية الحمراء - الوالي - كوبرى غمرة - الظاهر - سيد جلال - باب الشعرية - موسكى - باب الخلق - احمد ماهر - السيدة زينب - مستشفى المنيرة - ابو الريش - عابدين - المذبح - معهد الأورام - مستشفى 57357 - فم الخليج - مستشفى السلام - المحكمة الدستورية - المطبعة";

        dbContext.RouteStations.RemoveRange(dbContext.RouteStations);
        dbContext.BusRoutes.RemoveRange(dbContext.BusRoutes);
        dbContext.Buses.RemoveRange(dbContext.Buses);
        dbContext.Stations.RemoveRange(dbContext.Stations);
        await dbContext.SaveChangesAsync();

        // Load coordinates from CSV
        var stationCoords = new Dictionary<string, (double lat, double lon)>();
        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "route_stop_coordinates.csv");
        if (System.IO.File.Exists(csvPath))
        {
            var csvLines = await System.IO.File.ReadAllLinesAsync(csvPath);
            foreach (var csvLine in csvLines.Skip(1)) // Skip header
            {
                var columns = csvLine.Split(new[] { "\",\"", "\"" }, StringSplitOptions.RemoveEmptyEntries);
                if (columns.Length >= 5)
                {
                    var stopName = columns[2].Trim();
                    if (double.TryParse(columns[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(columns[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                    {
                        // Cairo boundary filter
                        if (lat >= 29.8 && lat <= 30.3 && lon >= 31.0 && lon <= 31.6)
                        {
                            stationCoords[stopName] = (lat, lon);
                        }
                    }
                }
            }
        }

        var lines = rawData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var allStationsDict = new Dictionary<string, Models.Station>();

        foreach (var line in lines)
        {
            var parts = line.Split(':', 2);
            if (parts.Length < 2) continue;
            var busNumber = parts[0].Trim();
            var stationsText = parts[1].Trim();
            var stationNames = stationsText.Split('-').Select(s => s.Trim()).ToList();

            var bus = new Models.Bus { Number = busNumber };
            dbContext.Buses.Add(bus);

            var route = new Models.BusRoute { RouteName = $"Route {busNumber}", Bus = bus };
            dbContext.BusRoutes.Add(route);

            int order = 1;
            var addedStationsInRoute = new HashSet<string>();

            foreach (var stName in stationNames)
            {
                if (string.IsNullOrWhiteSpace(stName)) continue;

                if (!allStationsDict.TryGetValue(stName, out var station))
                {
                    double lat = 30.0444;
                    double lon = 31.2357;

                    if (stationCoords.TryGetValue(stName, out var coords))
                    {
                        lat = coords.lat;
                        lon = coords.lon;
                    }

                    station = new Models.Station { Name = stName, Latitude = lat, Longitude = lon };
                    dbContext.Stations.Add(station);
                    allStationsDict[stName] = station;
                }

                if (!addedStationsInRoute.Add(stName)) continue;

                dbContext.RouteStations.Add(new Models.RouteStation
                {
                    BusRoute = route,
                    Station = station,
                    StationOrder = order++
                });
            }
        }

        await dbContext.SaveChangesAsync();
        return Ok("Seeded successfully");
    }
}
