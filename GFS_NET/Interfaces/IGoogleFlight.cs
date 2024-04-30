namespace GFS_NET.Interfaces
{
    public interface IGoogleFlight
    {
        void InitScraper(DateTime outbound, DateTime lastdate, int howManyDays, int flexDays, bool onlyWeekend, List<string> fromAirports, List<string> toAirports, string csvFileName);
        void StopScraper();
    }
}
