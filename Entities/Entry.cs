using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HatenaLib.Entities
{
    public class Entry
    {
        [JsonProperty("eid")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("screenshot")]
        public string ScreenshotUrl { get; set; }

        [JsonProperty("count")]
        public long BookmarkedCount { get; set; }

        [JsonProperty("bookmarks")]
        public Bookmark[] Bookmarks { get; set; }

        [JsonProperty("related")]
        public Entry[] RelatedEntries { get; set; }

        /// <summary>
        /// ブックマークコメント自身を表すURLを取得する
        /// <para>このURLに対してHatenaClient.GetStarsAsync()を使うとコメントへのスターが取得できる</para>
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="bookmark"></param>
        /// <returns></returns>
        public string GetBookmarkUrl(Bookmark bookmark)
        {
            if (bookmark == null)
            {
                throw new ArgumentNullException(nameof(bookmark));
            }

            var date = bookmark.Timestamp.ToString("yyyyMMdd");
            return $"http://b.hatena.ne.jp/{bookmark.UserName}/{date}#bookmark-{Id}";
        }

        /// <summary>
        /// エントリーに対して指定ユーザーが付けたブックマークコメントのURLを取得する
        /// <para>このURLに対してHatenaClient.GetStarsAsync()を使うとコメントへのスターが取得できる</para>
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="bookmark"></param>
        /// <returns></returns>
        public string GetBookmarkUrl(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(nameof(userName));
            }

            var bookmark = Bookmarks.FirstOrDefault(b => b.UserName == userName);
            if (bookmark == default(Entities.Bookmark))
            {
                throw new InvalidOperationException("invalid userName: " + userName);
            }

            return GetBookmarkUrl(bookmark);
        }
    }

    /// <summary>
    /// ホット・新着エントリーリスト項目
    /// </summary>
   public class EntriesListItem : UserBase
   {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonIgnore]
        private static Regex DomainParser = new Regex(@"https?:\/\/(?<domain>[^\/]+)\/?");

        [JsonIgnore]
        private string _Domain = null;

        [JsonIgnore]
        public string Domain { get => _Domain ?? (_Domain = DomainParser.Match(Url ?? string.Empty)?.Groups["domain"]?.Value) ?? string.Empty; }

        [JsonProperty("count")]
        public long BookmarkedCount { get; set; }

        [JsonProperty("snippet")]
        public string Description { get; set; }

        [JsonProperty("timestamp")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public string Encoded { get; set; }

        [JsonIgnore]
        private static Regex FaviconRegex = new Regex(@"http:\/\/cdn\-ak\.favicon\.st\-hatena\.com\/\?url\=[^""\s]+");

        [JsonIgnore]
        public string FaviconUrl
        {
            get
            {
                if (_FaviconUrl != null) { return _FaviconUrl; }
                if (string.IsNullOrEmpty(Encoded))
                {
                    return _FaviconUrl = string.Empty;
                }
                var match = FaviconRegex.Match(Encoded);
                if (match.Success)
                {
                    return _FaviconUrl = match.Value;
                }
                else
                {
                    return _FaviconUrl = string.Empty;
                }
            }
        }

        [JsonIgnore]
        private string _FaviconUrl = null;

        [JsonIgnore]
        private static Regex ThumbnailRegex = new Regex(@"https?:\/\/cdn\-ak\-scissors\.b\.st\-hatena\.com\/[^""\s]+");

        [JsonIgnore]
        public string ThumbnailUrl
        {
            get
            {
                if (_ThumbnailUrl != null) { return _ThumbnailUrl; }
                if (string.IsNullOrEmpty(Encoded))
                {
                    return _ThumbnailUrl = string.Empty;
                }
                var match = ThumbnailRegex.Match(Encoded);
                if (match.Success)
                {
                    return _ThumbnailUrl = match.Value;
                }
                else
                {
                    return _ThumbnailUrl = string.Empty;
                }
            }
        }

        [JsonIgnore]
        private string _ThumbnailUrl = null;
   }
}

