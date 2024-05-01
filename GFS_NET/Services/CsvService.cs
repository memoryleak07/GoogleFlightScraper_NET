using GFS_NET.Interfaces;
using GFS_NET.Objects;
using Microsoft.Data.Analysis;
using Microsoft.Extensions.Options;
using System.Globalization;

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
            var dataFrame = DataFrame.LoadCsv(filename: dataPath, cultureInfo: CultureInfo.InvariantCulture);

            // List of column names
            var columns = _googleOpt.Value.Xpaths.ToDictionary().Keys.ToList();

            columns.InsertRange(0, ["Departure", "Return"]);

            // Assign column names to the DataFrame
            for (int i = 0; i < columns.Count; i++)
            {
                dataFrame.Columns[i].SetName(columns[i]);
            }

            // Order by Price
            dataFrame = dataFrame.OrderBy("Price"); //TODO: for some reason it is added time to date strings. Why? Who asks?

            // Save the new data frame to a CSV file
            var newFilePath = Path.GetFullPath(@"sorted_"+csvFileName);

            DataFrame.SaveCsv(dataFrame, newFilePath, ',');

            _logger.Debug($"CSV file sorted successfully: {newFilePath}");

            return newFilePath;
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
