using OpenQA.Selenium.Chrome;

namespace GFS_NET.Interfaces
{
    public interface IScraperFactory
    {
        IScraper CreateScraper(int timeout, ChromeOptions options = null);
    }
}
