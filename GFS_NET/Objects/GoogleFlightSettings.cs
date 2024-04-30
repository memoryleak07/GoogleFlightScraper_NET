namespace GFS_NET.Objects
{
    public class GoogleFlightSettings
    {
        public class GoogleXpaths
        {
            public required string AirportCode { get; set; }
            public required string Price { get; set; }
            public required string Company { get; set; }
            public required string Stops { get; set; }
            public required string Duration { get; set; }

            public Dictionary<string, string> ToDictionary()
            {
                return new Dictionary<string, string>
                {
                    { nameof(AirportCode), AirportCode },
                    { nameof(Price), Price },
                    { nameof(Company), Company },
                    { nameof(Stops), Stops },
                    { nameof(Duration), Duration }
                };
            }
        }

        public required string BaseUrl { get; set; }
        public required GoogleXpaths Xpaths { get; set; }

    }
}
