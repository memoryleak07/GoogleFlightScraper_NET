using GFS_NET.Interfaces;
using GFS_NET.Objects;
using GFS_NET.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

// Create a new host application builder
var builder = Host.CreateApplicationBuilder(args);

// Register the IScraper interface with the ScraperService implementation
builder.Services.AddScoped<IScraper, ScraperService>();

// Build a configuration object using environment variables and JSON providers
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .AddJsonFile("chromeSettings.json")
    .AddJsonFile("googleFlightSettings.json") // Additional settings
    .AddEnvironmentVariables()
    .Build();

// Bind entire JSON files to IOptions for AppSettings and ChromeSettings
builder.Services.Configure<AppSettings>(config);
builder.Services.Configure<ChromeSettings>(config);

// Add new interfaces and configuration for GoogleFlight
builder.Services.AddScoped<IGoogleFlight, GoogleFlightService>();
builder.Services.Configure<GoogleFlightSettings>(config);

// Check values in appSettings file
int delta = config.GetValue<int>("Delta");
int flexdays = config.GetValue<int>("Flexdays");
DateTime outbound = DateTime.Parse(config.GetSection("Outbound").Value!);
DateTime lastDate = DateTime.Parse(config.GetSection("LastDate").Value!);

// Check for valid integer range inputs
if (delta <= 0 || flexdays <= 0)
{
    throw new Exception("Please check for valid range input in appSettings.json");
}

// Check for valid dates
if (outbound < DateTime.Now.Date || lastDate <= DateTime.Now.Date || outbound == lastDate)
{
    throw new Exception("Please check for valid date input in appSettings.json");
}

// Ask user if want to continue
Console.WriteLine(Environment.NewLine);
Console.WriteLine("AVOLOAVOLO.it TRIBUTE" + Environment.NewLine);
Console.WriteLine("Configure scraper parameters in 'appSettings.json' file." + Environment.NewLine);
Console.WriteLine("Do you want to continue? (Any key to continue, 'n' to exit)" + Environment.NewLine);

//string userInput = Console.ReadLine();

//if (userInput.ToLower() == "n")
//{
//    Environment.Exit(0);
//}



// Build the host
var host = builder.Build();

// Crate the scope
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get required service
        var scraper = services.GetRequiredService<IGoogleFlight>();

        // Provide the filename
        scraper.StartScraperLoop();

        // Quit the driver
        scraper.StopScraper();
    }
    catch (Exception ex) { throw new Exception(ex.Message); }

    finally { scope.Dispose(); }
}

