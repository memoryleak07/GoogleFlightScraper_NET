namespace GFS_NET.Interfaces
{
    public interface IGoogleFlight
    {
        void StartScraperLoop(DateTime firstDepartureDate, DateTime lastDepartureDate, int howManyDays, int flexDays, bool onlyWeekend, List<string> fromAirports, List<string> toAirports, string csvFileName);
        void StopScraper();
    }
}
