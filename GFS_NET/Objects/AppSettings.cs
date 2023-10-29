namespace GFS_NET.Objects
{
    public class AppSettings
    {
        public required List<string> FromAirports { get; set; }
        public required List<string> ToAirports { get; set; }
        public required DateTime FirstDepartureDate { get; set; }
        public required DateTime LastDepartureDate { get; set; }
        public required int HowManyDays { get; set; }
        public required int FlexDays { get; set; }
        public required bool OnlyWeekend { get; set; }
    }
}