using GFS_NET.Helpers;
using GFS_NET.Interfaces;
using GFS_NET.Objects;
using GFS_NET.Scraper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

static void GetAppSettings(IConfiguration config)
{
    config.Bind("AppSettings", new AppSettings());
}

// Create a service collection
// Create a service collection
var serviceProvider = new ServiceCollection()
    .Configure<AppSettings>(GetAppSettings)
    .AddSingleton<IAppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value)
    .AddSingleton<SettingsReader>()
    .BuildServiceProvider();

// Register the configuration
ConfigurationBuilder configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

serviceProvider.GetRequiredService<IConfiguration>().Bind(configuration);

// Create the AppSettings instance
var appSettings = serviceProvider.GetRequiredService<IAppSettings>();



//// Read settings.json file
//SettingsReader settingsReader = new ();
//AppSettings appSettings = settingsReader.ReadSettings();

//if (appSettings == null)
//{
//    throw new Exception("Error reading settings.json file");
//}


try
{
    GoogleFlightScraper googleFlightScraper = new ();
    // Declare vars
    DateTime startTime = DateTime.Now;
    string outFileCsv = startTime.ToString() + ".csv";
    Console.WriteLine($"Search start at {startTime}");

    // Start search
    googleFlightScraper.StartSearch(

        fromAirports: appSettings.Options.From,
        toAirports: appSettings.Options.To,
        outbound: appSettings.Options.Outbound,
        inbound: appSettings.Options.Outbound.AddDays(appSettings.Options.Delta),
        flexdays: appSettings.Options.Flexdays,
        lastdate: appSettings.Options.LastDate

    );
    DateTime endTime = DateTime.Now;
    Console.WriteLine($"Search end at {endTime}");
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
}