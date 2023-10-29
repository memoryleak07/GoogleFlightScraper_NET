namespace GFS_NET.Helpers
{
    public class CustomHelpers
    {
        public static string GoogleFlightUrlBuilder(string baseUrl, string from, string to, string outbound, string inbound)
        {
            // Replace placeholders in the base URL with actual values
            // Example url: "https://www.google.com/travel/flights?q=Flights+to+{to_}+from+{from_}+on+{outbound_}+through+{inbound_}"
            baseUrl = baseUrl.Replace("{from_}", from);
            baseUrl = baseUrl.Replace("{to_}", to);
            baseUrl = baseUrl.Replace("{outbound_}", outbound);
            baseUrl = baseUrl.Replace("{inbound_}", inbound);

            return baseUrl;
        }

        public static void AddListToCsvFile(List<string> resultList, string outFileNameCsv)
        {
            // Create a single string by joining the list of strings with a delimiter
            string line = string.Join(",", resultList);
            try
            {
                // Append the line to the CSV file with a newline character
                File.AppendAllText(outFileNameCsv, line + Environment.NewLine);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
