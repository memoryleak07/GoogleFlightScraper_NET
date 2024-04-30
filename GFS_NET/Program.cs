global using Serilog;
using GFS_NET.Interfaces;
using GFS_NET.Objects;
using GFS_NET.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ILogger>(new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"{DateTime.Now:yyyyMMddHHmmss}.log")
    .CreateLogger()
);

builder.Services.AddScoped<IScraper, ScraperService>();

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .AddJsonFile("chromeSettings.json")
    .AddJsonFile("googleFlightSettings.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.Configure<AppSettings>(config);
builder.Services.Configure<ChromeSettings>(config);
builder.Services.AddScoped<IGoogleFlight, GoogleFlightService>();
builder.Services.Configure<GoogleFlightSettings>(config);

DateTime outbound = DateTime.Parse(config.GetSection("FirstDepartureDate").Value!);
DateTime lastDate = DateTime.Parse(config.GetSection("LastDepartureDate").Value!);
int howManyDays = config.GetValue<int>("HowManyDays");
int flexDays = config.GetValue<int>("FlexDays");
bool onlyWeekend = config.GetValue<bool>("OnlyWeekend");
List<string> fromAirports = config.GetSection("FromAirports").Get<List<string>>()!;
List<string> toAirports = config.GetSection("ToAirports").Get<List<string>>()!;
if (howManyDays <= 0 || flexDays < 0)
{
    throw new Exception("Please check for valid range input in appSettings.json (HowManyDays, FlexDays)");
}
if (outbound < DateTime.Now.Date || lastDate <= DateTime.Now.Date || outbound == lastDate)
{
    throw new Exception("Please check for valid date input in appSettings.json (FirstDepartureDate, LastDepartureDate)");
}

//// Ask user if want to continue
//Console.WriteLine(Environment.NewLine);
//Console.WriteLine("AVOLOAVOLO.it TRIBUTE" + Environment.NewLine);
//Console.WriteLine("Configure scraper parameters in 'appSettings.json' file." + Environment.NewLine);
//Console.WriteLine("Do you want to continue? (Any key to continue, 'n' to exit)" + Environment.NewLine);

//string userInput = Console.ReadLine()!;

//if (userInput.ToLower() == "n")
//{
//    Environment.Exit(0);
//}

var host = builder.Build();
var csvFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var scraper = services.GetRequiredService<IGoogleFlight>();

        scraper.StartScraperLoop(outbound, lastDate, howManyDays, flexDays, onlyWeekend, fromAirports, toAirports, csvFileName);

        scraper.StopScraper();
    }
    catch (Exception ex) { throw new Exception(ex.Message); }

    finally { scope.Dispose(); }
}

