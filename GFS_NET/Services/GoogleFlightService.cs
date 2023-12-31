﻿using GFS_NET.Helpers;
using GFS_NET.Interfaces;
using GFS_NET.Objects;
using Microsoft.Extensions.Options;

namespace GFS_NET.Services
{
    public class GoogleFlightService : IGoogleFlight
    {
        private string _outFile;
        private readonly ILogger _log;
        private readonly IScraper _scraper;
        private readonly AppSettings _opt;
        private readonly GoogleFlightSettings _googleOpt;

        public GoogleFlightService(IScraper scraper, IOptions<AppSettings> appSettings, IOptions<GoogleFlightSettings> googleOpt, ILogger logger)
        {
            _log = logger;
            _scraper = scraper;
            _opt = appSettings.Value;
            _googleOpt = googleOpt.Value;
            _outFile = DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        }

        public void StopScraper()
        {
            _scraper.Dispose();
        }

        public void StartScraperLoop()
        {
            // Get options
            DateTime outbound = _opt.FirstDepartureDate;
            DateTime lastdate = _opt.LastDepartureDate;
            TimeSpan period = lastdate - outbound;

            _log.Information($"Writing results in: {_outFile}");
            _log.Information("ScraperLoop start");

            // Weekend inex
            int iWeekend = 0;

            foreach (string fromAirport in _opt.FromAirports)
            {
                foreach (string toAirport in _opt.ToAirports)
                {
                    for (int i = 0; i < period.Days; i++)
                    {
                        // Update dates
                        DateTime newOutbound = outbound.AddDays(i);
                        DateTime newInbound = newOutbound.AddDays(_opt.HowManyDays);

                        if (_opt.OnlyWeekend)
                        {
                            if (i == 0) { iWeekend = 0; }
                            newOutbound = DateTimeExtensions.NextWeekendDay(newOutbound.AddDays(iWeekend));
                            newInbound = newOutbound.AddDays(_opt.HowManyDays);
                            iWeekend += 5;
                        }

                        // Check if outbound equal to LastDate break loop
                        if (lastdate.Date == newOutbound.Date)
                        {
                            _log.Information("Break loop! LastDate is equal to Outbound.");
                            break;
                        }
                        // Scrape from inputs 
                        ScrapeFromInputs(
                            fromAirport,
                            toAirport,
                            newOutbound.ToString("yyyy-MM-dd"),
                            newInbound.ToString("yyyy-MM-dd")
                        );

                        // Iterate over flex days
                        if (_opt.FlexDays > 0)
                        {
                            for (int j = 0; j < _opt.FlexDays; j++)
                            {
                                // Update dates
                                newInbound = newOutbound.AddDays(j + 1 + _opt.HowManyDays);
                                // Scrape from inputs 
                                ScrapeFromInputs(
                                    fromAirport,
                                    toAirport,
                                    newOutbound.ToString("yyyy-MM-dd"),
                                    newInbound.ToString("yyyy-MM-dd")
                                );

                            }
                        }
                    }
                }
            }
            _log.Information("ScraperLoop end");
        }

        public void ScrapeFromInputs(string fromAirport, string toAirport, string outbound, string inbound)
        {
            string url = CustomHelpers.GoogleFlightUrlBuilder(_googleOpt.BaseUrl, fromAirport, toAirport, outbound, inbound);

            List<string>? results = _scraper.GetElementsFromXPathList(url, _googleOpt.Xpaths.ToList());

            if (results != null)
            {
                // Insert dates in results list
                results.InsertRange(0, new List<string> { outbound, inbound });

                // Print results (results is a List<string>)
                _log.Information(string.Join(" | ", results));

                // Add newResult to CSV file
                CustomHelpers.AddListToCsvFile(results, _outFile);
            }
        }
    }
}
