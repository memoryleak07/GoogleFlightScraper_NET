﻿
namespace GFS_NET.Interfaces
{
    public interface ICsvService
    {
        void AppendToCsvFile(List<string> results, string outboundDateStr, string inboundDateStr, string csvFileName);
        string SortCSVFile(string csvFileName);
    }
}