using GFS_NET.Interfaces;
using GFS_NET.Scraper;
using OpenQA.Selenium.Chrome;

public class ScraperFactory : IScraperFactory
{
    public IScraper CreateScraper(int timeout, ChromeOptions options = null)
    {
        if (options == null)
        {
            options = new ChromeOptions();
            // Set log level to suppress INFO messages
            options.AddArgument("--log-level=3");
            options.AddArgument("--disable-features=site-per-process");
            options.AddArgument("--disable-save-password-bubble");
            options.AddArgument("--disable-blink-features=AutomationControlled");
        }

        var webDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options);
        return new Scraper(webDriver, timeout);
    }
}
