using static GFS_NET.Objects.AppSettings;

namespace GFS_NET.Interfaces
{
    public interface IAppSettings
    {
        public OptionSettings? Options { get; set; }
        public XpathSettings? Xpaths { get; set; }
    }
}
