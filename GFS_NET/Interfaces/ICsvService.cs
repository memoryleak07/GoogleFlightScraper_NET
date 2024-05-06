
namespace GFS_NET.Interfaces
{
    public interface ICsvService
    {
        bool AppendToCsvFile(List<string> results, string outboundDateStr, string inboundDateStr, string csvFileName);
        (long, string) SortCSVFile(string csvFileName);
    }
}
