using GFS_NET.Helpers;
using GFS_NET.Interfaces;
using GFS_NET.Objects;
using Microsoft.Extensions.Options;

namespace GFS_NET.Services
{
    public class GoogleFlightService(IScraper scraper, IOptions<GoogleFlightSettings> googleOpt, ILogger logger, ICsvService csvService) : IGoogleFlight
    {
        private readonly ICsvService _csvService = csvService;
        private readonly ILogger _logger = logger;
        private readonly IScraper _scraper = scraper;
        private readonly string _baseURL = googleOpt.Value.BaseUrl;
        private readonly Dictionary<string, string> _xpathDict = googleOpt.Value.Xpaths.ToDictionary();

        public void StopScraper()
        {
            _scraper.Dispose();
        }

        public void InitScraper(
            DateTime outbound, DateTime lastdate, int howManyDays, int flexDays, bool onlyWeekend, List<string> fromAirports, List<string> toAirports, string csvFileName)
        {
            DateTime startTime = DateTime.Now;
            _logger.Information($"Loop start at {startTime}");

            int totCount = StartScraperLoop(outbound, lastdate, howManyDays, flexDays, onlyWeekend,fromAirports, toAirports, csvFileName);

            DateTime endTime = DateTime.Now;
            _logger.Information($"Loop end at {endTime}.");
            _logger.Information($"Total time elapsed: {(endTime - startTime).TotalHours} (hours)");
            _logger.Information($"Total search count: {totCount}");

            if (totCount > 0)
            {
                var (totRowCount, csvSorted) = _csvService.SortCSVFile(csvFileName);
                _logger.Information($"Result file raw: {csvFileName}.");
                _logger.Information($"Result file sorted by price: {csvSorted}.");
                _logger.Information($"Result file total row count: {totRowCount}.");
            }

            _logger.Information($"Goodbye, hope it helps.");
        }


        private string GoogleFlightUrlBuilder(string from, string to, string outbound, string inbound)
        {
            return $"{_baseURL.Replace("{TO}", to).Replace("{FROM}", from).Replace("{OUTBOUND}", outbound).Replace("{INBOUND}", inbound)}";
        }

        private bool ScrapeAndSaveInfo(string fromAirport, string toAirport, DateTime outbound, DateTime inbound, string csvFileName)
        {
            string outboundDateStr = outbound.ToString("yyyy-MM-dd");
            string inboundDateStr = inbound.ToString("yyyy-MM-dd");

            string url = GoogleFlightUrlBuilder(fromAirport, toAirport, outboundDateStr, inboundDateStr);

            _logger.Information($"Search: FROM {fromAirport} | TO {toAirport} | DEPARTURE {outboundDateStr} | RETURN {inboundDateStr}");

            List<string>? results = _scraper.GetElementsFromXpathDict(url, _xpathDict);

            if (results == null || results.Count == 0)
            {
                _logger.Information($"Search has no result.");
                return false;
            }

            _csvService.AppendToCsvFile(results, outboundDateStr, inboundDateStr, csvFileName);

            _logger.Information("Result: " + string.Join(" | ", results));

            return true;
        }

        private int StartScraperLoop(
            DateTime outbound, DateTime lastdate, int howManyDays, int flexDays, bool onlyWeekend, List<string> fromAirports, List<string> toAirports, string csvFileName)
        {
            TimeSpan period = lastdate - outbound;

            int totCount = 0;
            int iWeekend = 0;
            int consecutiveFailures = 0;

            foreach (string fromAirport in fromAirports)
            {
                foreach (string toAirport in toAirports)
                {
                    for (int i = 0; i < period.Days; i++)
                    {
                        if (consecutiveFailures >= 5)
                        {
                            _logger.Information($"Break! Too many consecutive search with no result. ({consecutiveFailures})");
                            break;
                        }

                        DateTime newOutbound = outbound.AddDays(i);
                        DateTime newInbound = newOutbound.AddDays(howManyDays);

                        if (onlyWeekend)
                        {
                            iWeekend = i == 0 ? 0 : iWeekend;
                            newOutbound = DateTimeExtensions.NextWeekendDay(newOutbound.AddDays(iWeekend));
                            newInbound = newOutbound.AddDays(howManyDays);
                            iWeekend += 5;
                        }

                        if (newOutbound.Date >= lastdate.Date)
                        {
                            _logger.Information($"Break! The departure date is equal to LastDepartureDate. ({lastdate:yyyy-MM-dd})");
                            break;
                        }

                        bool res = ScrapeAndSaveInfo(fromAirport, toAirport, newOutbound, newInbound, csvFileName);
                        consecutiveFailures = res == true ? 0 : ++consecutiveFailures;
                        totCount++;

                        if (flexDays > 0)
                        {
                            for (int j = 0; j < flexDays; j++)
                            {
                                newInbound = newOutbound.AddDays(j + 1 + howManyDays);
                                bool flexRes = ScrapeAndSaveInfo(fromAirport, toAirport, newOutbound, newInbound, csvFileName);
                                consecutiveFailures = flexRes == true ? 0 : ++consecutiveFailures;
                                totCount++;
                            }
                        }
                    }

                    consecutiveFailures = 0;
                }
            }

            return totCount;
        }
    }
}
