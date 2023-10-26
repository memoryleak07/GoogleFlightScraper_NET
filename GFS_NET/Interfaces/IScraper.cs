namespace GFS_NET.Interfaces
{
    public interface IScraper
    {
        void SetTimeout(int timeout);
        // Add other methods for web scraping as needed.
        void Dispose();
    }
}
