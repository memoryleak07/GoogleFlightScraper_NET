namespace GFS_NET.Interfaces
{
    public interface IGoogleFlight
    {
        void StartScraperLoop();
        void ScrapeFromInputs(string fromAirport, string toAirport, string outbound, string inbound);
        void StopScraper();
    }
}
