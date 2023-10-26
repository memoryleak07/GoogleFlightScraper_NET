using GFS_NET.Helpers;
using GFS_NET.Interfaces;
using GFS_NET.Objects;

namespace GFS_NET.Scraper
{
    public class GoogleFlightScraper
    {
        public void StartSearch(List<string> fromAirports, List<string> toAirports, DateTime outbound, DateTime inbound, int flexdays, DateTime lastdate)
        {
            // Init Scraper
            IScraperFactory factory = new ScraperFactory();
            IScraper scraper = factory.CreateScraper(timeout: 10, options: null);
            // Get Date range
            TimeSpan delta = inbound - outbound;
            TimeSpan period = lastdate - outbound;

            foreach (string fromAirport in fromAirports)
            {
                foreach (string toAirport in toAirports)
                {
                    for (int i = 0; i < period.Days; i++)
                    {
                        ScrapeFromInputs(
                            fromAirport,
                            toAirport,
                            outbound.ToString("yyyy-MM-dd"),
                            inbound.ToString("yyyy-MM-dd")
                            );
                    }
                }
            }
            // Quit the scraper
            scraper.Dispose();
        }

        public List<string> ScrapeFromInputs(string fromAirport, string toAirport, string outbound, string inbound)
        {
            List<string> results = new();
            string url = CustomHelpers.GFUrlBuilder();


            return results;
        }
    }
}
