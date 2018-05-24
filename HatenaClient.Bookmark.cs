using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib
{
    partial class HatenaClient
    {
        /// <summary>
        /// [OAuth] URLが指すページをブックマークする
        /// </summary>
        /// <param name="url"></param>
        /// <param name="comment"></param>
        /// <param name="isPrivate"></param>
        /// <returns></returns>
        public async Task<Entities.Bookmark> BookmarkAsync(
            string url,
            string comment = null,
            IEnumerable<string> tags = null,
            bool isPrivate = false,
            bool postTwitter = false,
            bool postFacebook = false,
            bool postMixi = false,
            bool postEvernote = false,
            bool sendMail = false)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) { throw new ArgumentException("invalid url: " + url, nameof(url)); }

            var apiUrl = HatenaClient.ApiBaseUrl + "/my/bookmark";
            if (comment == null) { comment = string.Empty; }

            string BoolToParam(bool b) => b ? "1" : "0";

            var rawDatas = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("url", url),
                new KeyValuePair<string,string>("comment", comment),
                new KeyValuePair<string,string>("private", BoolToParam(isPrivate)),
                new KeyValuePair<string,string>("post_twitter", BoolToParam(postTwitter)),
                new KeyValuePair<string,string>("post_facebook", BoolToParam(postFacebook) ),
                new KeyValuePair<string,string>("post_mixi", BoolToParam(postMixi)),
                new KeyValuePair<string,string>("post_evernote", BoolToParam(postEvernote)),
                new KeyValuePair<string, string>("send_mail", BoolToParam(sendMail)),
            };

            if (tags?.Any() == true)
            {
                rawDatas.AddRange(tags.Select(t => new KeyValuePair<string, string>("tags", t)).Take(10));
            }

            var data = new FormUrlEncodedContent(rawDatas);

            using (var client = MakeAuthorizedHttpClient())
            using (var response = await client.PostAsync(apiUrl, data))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Entities.Bookmark>(json);
                }
            }

            throw new HttpRequestException("bookmark failed. entry url: " + url);
        }

        /// <summary>
        /// [OAuth] ページを既にブックマークしている場合その情報を返す
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Entities.Bookmark> GetBookmarkAsync(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) { throw new ArgumentException("invalid url: " + url, nameof(url)); }
            try
            {
                var apiUrl = ApiBaseUrl + "/my/bookmark" + "?url=" + url;
                using (var client = MakeAuthorizedHttpClient())
                using (var response = await client.GetAsync(apiUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<Entities.Bookmark>(json);
                    }
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }

            throw new InvalidOperationException("the page has not been bookmarked yet. url: " + url);
        }

        /// <summary>
        /// [OAuth] URLが指すページをブックマークから除外する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBookmarkAsync(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) { throw new ArgumentException("invalid url: " + url, nameof(url)); }

            var apiUrl = ApiBaseUrl + "/my/bookmark";
            var data = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("url", url) });

            var req = new HttpRequestMessage(HttpMethod.Delete, apiUrl) { Content = data };
            using (var client = MakeAuthorizedHttpClient())
            using (var response = await client.SendAsync(req))
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.NoContent:
                        return true;

                    case System.Net.HttpStatusCode.NotFound:
                        return false;

                    default:
                        throw new HttpRequestException("invalid deletion. url: " + url);
                }
            }
        }

        /// <summary>
        /// ブックマークコメント自身のURLを取得する
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="bookmark"></param>
        /// <returns></returns>
        public static string GetBookmarkUrl(Entities.Entry entry, Entities.Bookmark bookmark)
        {
            return entry.GetBookmarkUrl(bookmark);
        }

        /// <summary>
        /// エントリーに対して指定ユーザーが付けたブックマークコメントのURLを取得する
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string GetBookmarkUrl(Entities.Entry entry, string userName)
        {
            return entry.GetBookmarkUrl(userName);
        }
    }
}
