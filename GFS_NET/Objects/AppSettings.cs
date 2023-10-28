namespace GFS_NET.Objects
{
    public class AppSettings
    {
        public required List<string> FromAirports { get; set; }
        public required List<string> ToAirports { get; set; }
        public required DateTime Outbound { get; set; }
        public required int Delta { get; set; }
        public required int Flexdays { get; set; }
        public required bool Weekend { get; set; }
        public required DateTime LastDate { get; set; }
    }
}