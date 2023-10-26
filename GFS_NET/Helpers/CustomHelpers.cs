using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFS_NET.Helpers
{
    public class CustomHelpers
    {
        public CustomHelpers() { }
        public string GFUrlBuilder(string baseUrl, string from, string to, string outbound, string inbound)
        {
            // Replace placeholders in the base URL with actual values
            baseUrl = baseUrl.Replace("{from_}", from);
            baseUrl = baseUrl.Replace("{to_}", to);
            baseUrl = baseUrl.Replace("{outbound_}", outbound);
            baseUrl = baseUrl.Replace("{inbound_}", inbound);

            return baseUrl;
        }
    }
}
