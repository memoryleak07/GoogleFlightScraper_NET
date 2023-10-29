﻿using GFS_NET.Helpers;
using GFS_NET.Interfaces;
using GFS_NET.Objects;
using Microsoft.Extensions.Options;

namespace GFS_NET.Services
{
    public class GoogleFlightService : IGoogleFlight
    {
        private string _outFile;
        private readonly IScraper _scraper;
        private readonly AppSettings _opt;
        private readonly GoogleFlightSettings _googleOpt;

        public GoogleFlightService(IScraper scraper, IOptions<AppSettings> appSettings, IOptions<GoogleFlightSettings> googleOpt)
        {
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
            DateTime outbound = _opt.Outbound;
            DateTime lastdate = _opt.LastDate;
            TimeSpan period = lastdate - outbound;

            Console.WriteLine("ScraperLoop start at: " + DateTime.Now.ToString());

            foreach (string fromAirport in _opt.FromAirports)
            {
                foreach (string toAirport in _opt.ToAirports)
                {
                    for (int i = 0; i < period.Days; i++)
                    {
                        // Update dates
                        DateTime newOutbound = outbound.AddDays(i);
                        // Get return flight date (outbound + delta)
                        DateTime inbound = newOutbound.AddDays(_opt.Delta);

                        // Check if outbound equal to LastDate break loop
                        if (lastdate.Date == newOutbound.Date)
                        {
                            Console.WriteLine("Break loop! LastDate is equal to Outbound");
                            break;
                        }
                        // Dates strings to my scraper
                        string outboundDate = newOutbound.ToString("yyyy-MM-dd");
                        string inboundDate = inbound.ToString("yyyy-MM-dd");

                        // Scrape from inputs 
                        ScrapeFromInputs(
                            fromAirport,
                            toAirport,
                            outboundDate,
                            inboundDate
                        );
                    }
                    //// Iterate over flex days
                    //if (_opt.Flexdays > 0)
                    //{
                    //    for (int j = 0; j < _opt.Flexdays; j++)
                    //    {
                    //        inboundDate = newOutbound.AddDays(j + 1 + _opt.Delta).ToString("yyyy-MM-dd");
                    //        ScrapeFromInputs(
                    //            fromAirport,
                    //            toAirport,
                    //            outboundDate,
                    //            inboundDate
                    //        );

                    //    }
                    //}
                }
            }
            Console.WriteLine("ScraperLoop end at: " + DateTime.Now.ToString());
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
                Console.WriteLine(string.Join(" ", results));

                // Add newResult to CSV file
                CustomHelpers.AddListToCsvFile(results, _outFile);
            }
        }
    }
}