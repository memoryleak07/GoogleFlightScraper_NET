namespace GFS_NET.Interfaces
{
    public interface IScraper
    {
        void Dispose();
        List<string> GetElementsFromXpathDict(string url, Dictionary<string, string> xpaths);
    }
}
