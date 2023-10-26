using GFS_NET.Interfaces;
using OpenQA.Selenium;
namespace GFS_NET.Scraper

{
    public class Scraper : IScraper
    {
        private IWebDriver driver;
        private int timeout;

        public Scraper(IWebDriver webDriver, int timeout)
        {
            driver = webDriver;
            this.timeout = timeout;
        }

        public void SetTimeout(int timeout)
        {
            this.timeout = timeout;
        }

        // Add more methods and properties as needed for your scraping logic.

        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}
