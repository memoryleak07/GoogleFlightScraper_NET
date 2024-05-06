using GFS_NET.Interfaces;
using GFS_NET.Objects;
using Microsoft.Data.Analysis;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace GFS_NET.Services
{
    public class CsvService(ILogger logger, IOptions<GoogleFlightSettings> googleOpt) : ICsvService
    {
        private readonly ILogger _logger = logger;
        private readonly IOptions<GoogleFlightSettings> _googleOpt = googleOpt;

        public (long, string) SortCSVFile(string csvFileName)
        {
            try
            {
                var dataPath = Path.GetFullPath(csvFileName);
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
                //TODO: for some reason it is added time to date strings.
                dataFrame = dataFrame.OrderBy("Price");

                // Save the new data frame to a CSV file
                var newFilePath = Path.GetFullPath(@"sorted_"+csvFileName);
                DataFrame.SaveCsv(dataFrame, newFilePath, ',');

                var totalRowCount = dataFrame.Rows.Count;
                _logger.Debug($"CSV file sorted successfully: {newFilePath}");
                _logger.Debug($"Total row count: {totalRowCount}");

                return (totalRowCount, newFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw new Exception(ex.Message, ex);
            }
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
