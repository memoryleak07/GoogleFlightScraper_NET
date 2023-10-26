using GFS_NET.Interfaces;

namespace GFS_NET.Objects
{
    public class AppSettings : IAppSettings
    {
        public class OptionSettings
        {
            public string BaseUrl { get; set; }
            public List<string> From { get; set; }
            public List<string> To { get; set; }
            public DateTime Outbound { get; set; }
            public int Delta { get; set; }
            public int Flexdays { get; set; }
            public bool Weekend { get; set; }
            public DateTime LastDate { get; set; }
            public bool FastMode { get; set; }
            public int Timeout { get; set; }
        }
        public class XpathSettings
        {
            public string AirportCode { get; set; }
            public string Price { get; set; }
            public string Company { get; set; }
            public string Type { get; set; }
            public string Duration { get; set; }
            public string Stops { get; set; }
            public string ClickFirstSorted { get; set; }
            public string NotFound { get; set; }
            public string NotFoundMessage { get; set; }
        }

        public OptionSettings? Options { get; set; }
        public XpathSettings? Xpaths { get; set; }
    }
}
