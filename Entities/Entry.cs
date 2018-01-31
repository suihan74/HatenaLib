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
        public string Domain { get => DomainParser.Match(Url ?? string.Empty)?.Groups["domain"]?.Value ?? string.Empty; }

        [JsonProperty("count")]
        public long BookmarkedCount { get; set; }

        [JsonProperty("snippet")]
        public string Description { get; set; }

        [JsonProperty("timestamp")]
        public DateTime CreatedAt { get; set; }
   }
}

