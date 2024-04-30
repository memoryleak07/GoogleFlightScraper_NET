using GFS_NET.Helpers;
using GFS_NET.Interfaces;
using GFS_NET.Objects;
using Microsoft.Extensions.Options;

namespace GFS_NET.Services
{
    public class GoogleFlightService(IScraper scraper, IOptions<GoogleFlightSettings> googleOpt, ILogger logger) : IGoogleFlight
    {
        private readonly ILogger _logger = logger;
        private readonly IScraper _scraper = scraper;
        private readonly string _baseURL = googleOpt.Value.BaseUrl;
        private readonly Dictionary<string, string> _xpathDict = googleOpt.Value.Xpaths.ToDictionary();

        public void StopScraper()
        {
            _scraper.Dispose();
        }

        private string GoogleFlightUrlBuilder(string from, string to, string outbound, string inbound)
        {
            string baseUrl = _baseURL;
            baseUrl = baseUrl.Replace("{from_}", from);
            baseUrl = baseUrl.Replace("{to_}", to);
            baseUrl = baseUrl.Replace("{outbound_}", outbound);
            baseUrl = baseUrl.Replace("{inbound_}", inbound);
            return baseUrl;
        }

        private void ScrapeAndSaveInfo(string fromAirport, string toAirport, DateTime outbound, DateTime inbound, string csvFileName)
        {
            string outboundDateStr = outbound.ToString("yyyy-MM-dd");
            string inboundDateStr = inbound.ToString("yyyy-MM-dd");

            string url = GoogleFlightUrlBuilder(fromAirport, toAirport, outboundDateStr, inboundDateStr);

            _logger.Information($"Search: FROM {fromAirport} | TO {toAirport} | DEPARTURE {outboundDateStr} | RETURN {inboundDateStr}");

            List<string>? results = _scraper.GetElementsFromXpathDict(url, _xpathDict);

            if (results == null)
            {
                _logger.Information($"Search has no result.");
                return;
            }

            results.InsertRange(0, [outboundDateStr, inboundDateStr]);

            string line = string.Join(",", results);

            File.AppendAllText(csvFileName, line + Environment.NewLine);

            _logger.Information("Result: " + string.Join(" | ", results));
        }

        public void StartScraperLoop(
            DateTime outbound, DateTime lastdate, int howManyDays, int flexDays, bool onlyWeekend, List<string> fromAirports, List<string> toAirports, string csvFileName)
        {
            TimeSpan period = lastdate - outbound;

            int iWeekend = 0;

            foreach (string fromAirport in fromAirports)
            {
                foreach (string toAirport in toAirports)
                {
                    for (int i = 0; i < period.Days; i++)
                    {
                        DateTime newOutbound = outbound.AddDays(i);
                        DateTime newInbound = newOutbound.AddDays(howManyDays);

                        if (onlyWeekend)
                        {
                            iWeekend = i == 0 ? 0 : iWeekend;
                            newOutbound = DateTimeExtensions.NextWeekendDay(newOutbound.AddDays(iWeekend));
                            newInbound = newOutbound.AddDays(howManyDays);
                            iWeekend += 5;
                        }

                        if (lastdate.Date == newOutbound.Date)
                        {
                            _logger.Information("Break loop! Last date is equal to Outbound.");
                            break;
                        }

                        ScrapeAndSaveInfo(fromAirport, toAirport, newOutbound, newInbound, csvFileName);

                        if (flexDays > 0)
                        {
                            for (int j = 0; j < flexDays; j++)
                            {
                                newInbound = newOutbound.AddDays(j + 1 + howManyDays);

                                ScrapeAndSaveInfo(fromAirport, toAirport, newOutbound, newInbound, csvFileName);
                            }
                        }
                    }
                }
            }

        }
    }
}
