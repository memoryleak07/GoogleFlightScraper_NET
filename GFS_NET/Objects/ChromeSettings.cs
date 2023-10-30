namespace GFS_NET.Objects
{
    public class ChromeSettings
    {
        public required int Timeout { get; set; }
        public required string GoogleBaseUrl { get; set; }
        public required List<string> DriverOptions { get; set; }
        public required string AcceptCookieBtn { get; set; }
        public required bool RandomizeUserAgent { get; set; }
        public required List<string> UserAgents { get; set; }
        public required bool RandomizeReferer { get; set; }
        public required List<string> Referers { get; set; }
    }
}
