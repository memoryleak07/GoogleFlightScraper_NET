namespace GFS_NET.Interfaces
{
    public interface IScraper
    {
        void Dispose();
        string? GetElement(string url, string xpath);
        List<string> GetElementsFromXPathList(string url, List<string> xpathList);
    }
}
