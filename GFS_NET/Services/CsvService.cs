using GFS_NET.Interfaces;
using GFS_NET.Objects;
using Microsoft.Data.Analysis;
using Microsoft.Extensions.Options;

namespace GFS_NET.Services
{
    public class CsvService : ICsvService
    {
        private readonly ILogger _logger;
        private readonly IOptions<GoogleFlightSettings> _googleOpt;

        public CsvService(ILogger logger, IOptions<GoogleFlightSettings> googleOpt)
        {
            _logger = logger;
            _googleOpt = googleOpt;
        }

        public string SortCSVFile(string csvFileName)
        {
            // Define data path
            var dataPath = Path.GetFullPath(@"test.csv");

            // Load the data into the data frame
            var dataFrame = DataFrame.LoadCsv(dataPath);

            // Preview
            dataFrame.Info();

            // Summary
            dataFrame.Description();

            string csvFileNameSorted = "sorted_" + csvFileName;

            _logger.Debug($"CSV file sorted successfully: {csvFileNameSorted}");

            return csvFileNameSorted;
        }

        public void AppendToCsvFile(List<string> results, string outboundDateStr, string inboundDateStr, string csvFileName)
        {

            var formattedList = results
                .Select(s => s.Replace("€", "").Replace("$", "").Replace(",", "."))
                .ToList();

            formattedList.InsertRange(0, [outboundDateStr, inboundDateStr]);

            string line = string.Join(",", formattedList);

            File.AppendAllText(csvFileName, line + Environment.NewLine);
        }
    }
}
