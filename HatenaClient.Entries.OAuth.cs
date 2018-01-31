using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib
{
    public partial class HatenaClient
    {
        /// <summary>
        /// [OAuth] ユーザーフィードを取得
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Task<IEnumerable<Entities.EntriesListItem>> GetUserEntriesAsync(long? offset = null)
        {
            return HatenaClient.GetUserEntriesAsync(Account.Name, offset);
        }

        /// <summary>
        /// [OAuth] エントリーフィードを取得
        /// </summary>
        /// <param name="category"></param>
        /// <param name="mode"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Task<IEnumerable<Entities.EntriesListItem>> GetEntriesAsync(Category category, EntriesListMode mode, long? offset = null)
        {
            if (category == Category.UserFeeds)
            {
                return GetUserEntriesAsync(offset);
            }
            else
            {
                return GetEntriesAsync(category, mode);
            }
        }

        /// <summary>
        /// ブックマーク済みのエントリーを検索する
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Entities.EntriesListItem>> SearchBookmarkedEntriesAsync(string queue, long limit = 100)
        {
            if (limit > 100) { limit = 100; }

            var apiUrl = $"{BaseUrl}/{Account.Name}/search/json";
            apiUrl += "?q=" + Uri.EscapeDataString(queue) + "&limit=" + limit;

            using (var client = MakeAuthorizedHttpClient())
            {
                var json = await client.GetStringAsync(apiUrl);
                var result = JsonConvert.DeserializeObject<SearchResult>(json);
                return result.Bookmarks.Select(b =>
                {
                    b.Entry.CreatedAt = DateTimeOffset.FromUnixTimeSeconds(b.Timestamp).LocalDateTime;
                    return b.Entry;
                });
            }
        }
    }

    /// <summary>
    /// 検索結果
    /// </summary>
    class SearchResult
    {
        public class SearchResultBookmark
        {
            [JsonProperty("entry")]
            public Entities.EntriesListItem Entry { get; set; }

            [JsonProperty("comment")]
            public string Comment { get; set; }

            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }
        }

        [JsonProperty("bookmarks")]
        public SearchResultBookmark[] Bookmarks { get; set; }
    }
}
