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
        private DataFrame _dataFrame;

        public CsvService(ILogger logger, IOptions<GoogleFlightSettings> googleOpt, DataFrame dataFrame)
        {
            _logger = logger;
            _googleOpt = googleOpt;
            _dataFrame = dataFrame;
        }

        public void OpenCsvFile(string csvFileName)
        {
            try
            {
                var dataPath = Path.GetFullPath(csvFileName);
                _dataFrame = DataFrame.LoadCsv(filename: dataPath, cultureInfo: CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public int GetCsvFileRowCount(string csvFileName)
        {
            OpenCsvFile(csvFileName);
            return _dataFrame.Rows.Count();
        }

        public string SortCSVFile(string csvFileName)
        {
            OpenCsvFile(csvFileName);
            // List of column names
            var columns = _googleOpt.Value.Xpaths.ToDictionary().Keys.ToList();

            columns.InsertRange(0, ["Departure", "Return"]);

            // Assign column names to the DataFrame
            for (int i = 0; i < columns.Count; i++)
            {
                _dataFrame.Columns[i].SetName(columns[i]);
            }

            // Order by Price
            _dataFrame = _dataFrame.OrderBy("Price"); //TODO: for some reason it is added time to date strings. Why? Who asks?

            // Save the new data frame to a CSV file
            var newFilePath = Path.GetFullPath(@"sorted_"+csvFileName);

            DataFrame.SaveCsv(_dataFrame, newFilePath, ',');

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
