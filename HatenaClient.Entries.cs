using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HatenaLib
{
    /// <summary>
    /// カテゴリー
    /// </summary>
    public enum Category
    {
        All,
        Social,
        Economics,
        Life,
        Knowledge,
        It,
        Fun,
        Entertainment,
        Game,

        UserFeeds,
        Tags,
        [Obsolete] MyHotEntries,
        Favorites,
        Search
    }

    /// <summary>
    /// ホット/新着
    /// </summary>
    public enum EntriesListMode
    {
        Hot,
        Recent
    }

    public partial class HatenaClient
    {
        /// <summary>
        /// エントリーリストを取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"/>
        private static async Task<IEnumerable<Entities.EntriesListItem>> GetEntriesImplAsync(string url, Dictionary<string, string> headers = null)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) { throw new ArgumentException("invalid url: " + url); }

            // キャッシュ回避
            if (url.Contains("?"))
            {
                url += "&" + DateTime.Now.Ticks;
            }
            else
            {
                url += "?" + DateTime.Now.Ticks;
            }

            var xml = await GetXDocumentAsync(url, headers);

            XNamespace xmlns = "http://www.w3.org/2000/xmlns/";
            XNamespace ns = xml.Root.Attribute("xmlns").Value;
            XNamespace dc = xml.Root.Attribute(xmlns + "dc").Value;
            XNamespace hatena = xml.Root.Attribute(xmlns + "hatena").Value;
            XNamespace content = xml.Root.Attribute(xmlns + "content").Value;

            var result = xml.Descendants(ns + "item")
                .Select(item =>
                {
                    return new Entities.EntriesListItem()
                    {
                        BookmarkedCount = long.Parse(item.Element(hatena + "bookmarkcount").Value),
                        CreatedAt = DateTime.Parse(item.Element(dc + "date").Value),
                        Description = item.Element(ns + "description").Value,
                        Encoded = item.Element(content + "encoded").Value,
                        Title = item.Element(ns + "title").Value,
                        Url = item.Element(ns + "link").Value,
                        UserName = item.Element(dc + "creator")?.Value ?? string.Empty,
                    };
                })
                .ToList();

            return result;
        }

        /// <summary>
        /// ホット・新着エントリ一覧を取得する
        /// </summary>
        /// <param name="category"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Entities.EntriesListItem>> GetEntriesAsync(Category category, EntriesListMode mode)
        {
            var target = mode == EntriesListMode.Hot ? "hotentry" : "entrylist";
            var endpoint = GetCategoryEndPoint(category);
            var url = $"{BaseUrl}/{target}{endpoint}";
            return GetEntriesImplAsync(url);
        }

        /// <summary>
        /// エントリーを検索
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"/>
        private static Task<IEnumerable<Entities.EntriesListItem>> SearchEntriesAsync(
            string mode,
            string query,
            EntriesListMode order,
            long threshold,
            DateTime dateBegin,
            DateTime dateEnd,
            bool safe)
        {
            if (!(mode == "text" || mode == "tag" || mode == "title")) { throw new ArgumentException("invalid mode: " + mode); }

            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException(nameof(query));
            }

            var url = $"{BaseUrl}/search/{mode}?q={query}&mode=rss&threshold={threshold}&safe={(safe ? "on" : "off")}";
            switch (order)
            {
                case EntriesListMode.Hot:
                    url += "&sort=popular";
                    break;

                case EntriesListMode.Recent:
                    url += "&sort=recent";
                    break;

                default: throw new NotImplementedException();
            }

            if (dateBegin != default(DateTime))
            {
                url += $"&date_begin={dateBegin.ToString("yyyy-MM-dd")}";
            }

            if (dateEnd != default(DateTime))
            {
                url += $"&date_end={dateEnd.ToString("yyyy-MM-dd")}";
            }

            return GetEntriesImplAsync(url);
        }

        /// <summary>
        /// キーワードを用いてエントリーを検索
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"/>
        public static Task<IEnumerable<Entities.EntriesListItem>> SearchEntriesWithKeywordAsync(
            string keyword,
            EntriesListMode mode = EntriesListMode.Recent,
            long threshold = 3,
            DateTime dateBegin = default(DateTime),
            DateTime dateEnd = default(DateTime),
            bool safe = false)
        {
            return SearchEntriesAsync("text", keyword, mode, threshold, dateBegin, dateEnd, safe);
        }

        /// <summary>
        /// タグを用いてエントリーを検索
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"/>
        public static Task<IEnumerable<Entities.EntriesListItem>> SearchEntriesWithTagAsync(
            string tag,
            EntriesListMode mode = EntriesListMode.Recent,
            long threshold = 3,
            DateTime dateBegin = default(DateTime),
            DateTime dateEnd = default(DateTime),
            bool safe = false)
        {
            return SearchEntriesAsync("tag", tag, mode, threshold, dateBegin, dateEnd, safe);
        }

        /// <summary>
        /// タイトルを用いてエントリーを検索
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"/>
        public static Task<IEnumerable<Entities.EntriesListItem>> SearchEntriesWithTitleAsync(
            string title,
            EntriesListMode mode = EntriesListMode.Recent,
            long threshold = 3,
            DateTime dateBegin = default(DateTime),
            DateTime dateEnd = default(DateTime),
            bool safe = false)
        {
            return SearchEntriesAsync("title", title, mode, threshold, dateBegin, dateEnd, safe);
        }

        /// <summary>
        /// 特定サイトのエントリーを検索
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Entities.EntriesListItem>> SearchEntriesWithUrlAsync(
            string targetUrl,
            EntriesListMode mode = EntriesListMode.Recent)
        {
            if (string.IsNullOrWhiteSpace(targetUrl))
            {
                throw new ArgumentException("invalid url", nameof(targetUrl));
            }

            var sort = mode == EntriesListMode.Recent ? "recent" : "count";
            var url = $"{BaseUrl}/entrylist?url={targetUrl}&mode=rss&sort={sort}";
            return GetEntriesImplAsync(url);
        }

        /// <summary>
        /// ユーザーフィードを取得
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Entities.EntriesListItem>> GetUserEntriesAsync(string userName, DateTime date = default(DateTime), long? offset = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("invalid username", nameof(userName));
            }

            var url = $"{BaseUrl}/{userName}/rss";

            if (date != default(DateTime))
            {
                url += $"?date={date.ToString("yyyyMMdd")}";
            }

            if (offset is long of)
            {
                url += (date == default(DateTime) ? "?" : "&") + $"of={of * 20}";
            }

            return GetEntriesImplAsync(url);
        }

        /// <summary>
        /// 特定タグが付けられたユーザーフィードを取得
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="tag"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Entities.EntriesListItem>> GetUserTaggedEntriesAsync(string userName, string tag, long? offset = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("invalid username", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException("invalid tag", nameof(tag));
            }

            var url = $"{BaseUrl}/{userName}/rss?tag={tag}";
            if (offset is long of)
            {
                url += $"&of={of * 20}";
            }
            return GetEntriesImplAsync(url);
        }

        /// <summary>
        /// 特定ユーザーのお気に入りユーザーのブクマ一覧を取得する
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Entities.EntriesListItem>> GetFavoriteUsersEntriesAsync(string userName)
        {
            var url = $"{BaseUrl}/{userName}/favorite.rss";
            return await GetEntriesImplAsync(url);
        }

        private static string GetCategoryEndPoint(Category category)
        {
            switch (category)
            {
                case Category.All: return "?mode=rss";
                case Category.Economics: return "/economics.rss";
                case Category.Entertainment: return "/entertainment.rss";
                case Category.Fun: return "/fun.rss";
                case Category.Game: return "/game.rss";
                case Category.It: return "/it.rss";
                case Category.Knowledge: return "/knowledge.rss";
                case Category.Life: return "/life.rss";
                case Category.Social: return "/social.rss";
                default: throw new ArgumentException($"invalid category: {category.ToString("G")}", nameof(category));
            }
        }

        /// <summary>
        /// エントリーの詳細情報を取得
        /// </summary>
        /// <param name="url"></param>
        /// <param name="liteMode"></param>
        /// <returns></returns>
        public static Task<Entities.Entry> GetEntryAsync(string url, bool liteMode = false)
        {
            var apiUrl = $"{BaseUrl}/entry/json{(liteMode ? "lite" : "")}/?url={(Uri.EscapeDataString(url))}";
            // キャッシュ回避
            apiUrl += "&" + DateTime.Now.Ticks;
            return GetJsonObjectAsync<Entities.Entry>(apiUrl);
        }
    }
}
