using HatenaLib.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib
{
    public partial class HatenaClient
    {
        /// <summary>
        /// はてなスター - http://s.hatena.ne.jp
        /// </summary>
        public static readonly string StarBaseUrl = "http://s.hatena.ne.jp";

        /// <summary>
        /// URLが指すページのはてなスター情報を取得
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Entities.StarEntry>> GetStarsAsync(IEnumerable<string> urls)
        {
            var apiUrl = $"{StarBaseUrl}/entry.json?";
            var cur = apiUrl + string.Join("&", urls.Select(u => $"uri={(Uri.EscapeDataString(u))}"));

            // 2048～2096字を超えるURLが使用できないため，リクエストを複数回に分ける
            if (cur.Length > 2000)
            {
                var tasks = new List<Task<Entities.StarEntries>>();
                do
                {
                    var tmp = cur.Substring(0, 2000);
                    var targetUrl = tmp.Substring(0, tmp.LastIndexOf("&"));
                    cur = apiUrl + tmp.Substring(tmp.LastIndexOf("&") + 1) + cur.Substring(2000);

                    tasks.Add(GetJsonObjectAsync<Entities.StarEntries>(targetUrl));
                }
                while (cur.Length > 2000);

                tasks.Add(GetJsonObjectAsync<Entities.StarEntries>(cur));
                await Task.WhenAll(tasks);

                var list = new List<Entities.StarEntry>();
                foreach (var t in tasks)
                {
                    list.AddRange(t.Result.Entries);
                }
                return list;
            }
            else
            {
                var result = await GetJsonObjectAsync<Entities.StarEntries>(cur);
                return result.Entries;
            }
        }

        /// <summary>
        /// URLが指すページのはてなスター情報を取得
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static async Task<Entities.StarEntry> GetStarsAsync(string url)
        {
            var result = await GetStarsAsync(new[] { url });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// URLが指すページに付いたはてなスターを表す画像を取得
        /// </summary>
        /// <param name="url"></param>
        /// <param name="bgColor"></param>
        /// <param name="doubleSize"></param>
        /// <returns></returns>
        public static async Task<Stream> GetStarsImageAsync(string url, System.Drawing.Color? bgColor = null, bool doubleSize = false)
        {
            var apiUrl = "http://s.st-hatena.com/entry.count.image?uri=" + Uri.EscapeDataString(url);
            if (bgColor is System.Drawing.Color c)
            {
                var R = c.R.ToString("X2");
                var G = c.G.ToString("X2");
                var B = c.B.ToString("X2");
                var code = "#" + R + G + B;
                apiUrl += $"&bg=#{code}";
            }
            if (doubleSize)
            {
                apiUrl += "&q=1";
            }

            var content = await GetAsync(apiUrl);
            return await content.ReadAsStreamAsync();
        }
    }
}
