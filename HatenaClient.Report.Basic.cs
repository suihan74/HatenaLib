using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib
{
    public enum ReportCategory
    {
        Spam = 0,
        Slander,
        PrivacyInfringement,
        CrimeNotice,
        Discrimination,
        Other
    }

    public partial class HatenaClient
    {
        public async Task Report(Entities.Entry entry, string userName, ReportCategory category, string comment = "")
        {
            var url = $"{BaseUrl}/entry.report";
            var contents = new Dictionary<string, string>
            {
                {"rks", RksForBookmark},
                {"eid", entry.Id.ToString()},
                {"name", userName},
                {"category", category.GetHashCode().ToString()},
                {"comment", comment}
            };
            await PostAsync(url, contents, GetCookieHeader());
        }
    }
}
