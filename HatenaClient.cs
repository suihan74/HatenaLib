using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

using HatenaLib.Entities;

namespace HatenaLib
{
    public partial class HatenaClient : BaseHttpClient
    {
        /// <summary>
        /// はてブ - http://b.hatena.ne.jp
        /// </summary>
        public static readonly string BaseUrl = "http://b.hatena.ne.jp";
        /// <summary>
        /// 要OAuth認証はてブ機能 - http://api.b.hatena.ne.jp/1
        /// </summary>
        public static readonly string ApiBaseUrl = "http://api.b.hatena.ne.jp/1";

        /// <summary>
        /// アクセストークン
        /// </summary>
        private AsyncOAuth.AccessToken AccessToken { get; }

        /// <summary>
        /// 自身のアカウント
        /// </summary>
        public Entities.Account Account { get; set; }

        /// <summary>
        /// 認証情報
        /// </summary>
        public Entities.Auth Auth { get; }

        /// <summary>
        /// 非表示ユーザー集合
        /// </summary>
        public HashSet<string> IgnoreUsersSet { get; private set; } = new HashSet<string>();

        #region Ctor

        public HatenaClient(Auth auth)
        {
            AccessToken = new AsyncOAuth.AccessToken(auth.Token, auth.TokenSecret);
            Auth = auth;
        }

        #endregion

        /// <summary>
        /// 認証情報を埋め込んだHttpClientを取得
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private HttpClient MakeAuthorizedHttpClient(IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            var client = AsyncOAuth.OAuthUtility.CreateOAuthClient(
                Auth.App.ConsumerKey,
                Auth.App.ConsumerSecret,
                AccessToken,
                headers);
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            return client;
        }

        #region Static Methods

        /// <summary>
        /// 特定ユーザーのタグ一覧を取得する
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Entities.Tag>> GetUserTagsAsync(string username)
        {
            var url = $"{BaseUrl}/{username}/tags.json&{DateTime.Now.Ticks}";
            var response = await GetJsonObjectAsync<Entities.TagsResponse>(url);
            if (response.Status != HttpStatusCode.OK) { throw new HttpRequestException("failed to get tags"); }

            var result = new Entities.Tag[response.Count];
            int i = 0;
            foreach(var tag in response.Tags)
            {
                tag.Value.Text = tag.Key;
                result[i++] = tag.Value;
            }
            return result;
        }

        /// <summary>
        /// エントリーURLからブコメページのURLを取得
        /// </summary>
        /// <param name="entryUrl"></param>
        /// <returns></returns>
        public static string GetBookmarkPageUrl(string entryUrl)
        {
            try
            {
                return GetBookmarkPageUrl(new Uri(entryUrl));
            }
            catch (Exception)
            {
                throw new ArgumentException($"invalid url: {entryUrl}", nameof(entryUrl));
            }
        }

        /// <summary>
        /// エントリーURLからブコメページのURLを取得
        /// </summary>
        /// <param name="entryUri"></param>
        /// <returns></returns>
        public static string GetBookmarkPageUrl(Uri entryUri)
        {
            var srcUrl = entryUri.AbsoluteUri.Replace(entryUri.Scheme + "://", "");
            return "http://b.hatena.ne.jp/entry/" + (entryUri.Scheme == "https" ? "s/" : "") + srcUrl;
        }

        /// <summary>
        /// ブコメページURLからエントリーURLを取得
        /// </summary>
        /// <param name="commentsPageUrl"></param>
        /// <returns></returns>
        public static string GetEntryPageUrl(string commentsPageUrl)
        {
            try
            {
                return GetEntryPageUrl(new Uri(commentsPageUrl));
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ArgumentException($"invalid url: {commentsPageUrl}", nameof(commentsPageUrl));
            }
        }

        /// <summary>
        /// ブコメページURLからエントリーURLを取得
        /// </summary>
        /// <param name="bookmarkCommentPageUrl"></param>
        /// <returns></returns>
        public static string GetEntryPageUrl(Uri commentsPageUri)
        {
            var hatenaBaseUrl = "http://b.hatena.ne.jp/entry/";
            if (!commentsPageUri.AbsoluteUri.StartsWith(hatenaBaseUrl))
            {
                throw new ArgumentException("URL is not a comment page: " + commentsPageUri.AbsoluteUri, nameof(commentsPageUri));
            }

            var destUrl = commentsPageUri.AbsoluteUri.Replace(hatenaBaseUrl, "");
            if (destUrl.StartsWith("s/"))
            {
                destUrl = "https://" + destUrl.Substring(2);
            }
            else
            {
                destUrl = "http://" + destUrl;
            }

            return destUrl;
        }

        /// <summary>
        /// ブコメURLからエントリーIDを抽出
        /// （ブコメURL例: "http://b.hatena.ne.jp/suihan74/20180513#bookmark-363996620"）
        /// </summary>
        /// <param name="bookmarkUrl"></param>
        /// <returns></returns>
        public static long GetEntryId(string bookmarkUrl)
        {
            var regex = new Regex($@"http:\/\/b\.hatena\.ne\.jp\/(?<username>\w+)\/(?<date>\d+)\#bookmark\-(?<eid>\d+)$");
            var match = regex.Match(bookmarkUrl);
            var eidGroup = match.Groups["eid"];

            if (eidGroup.Success && long.TryParse(eidGroup.Value, out var eid))
            {
                return eid;
            }

            throw new ArgumentException("invalid url: " + bookmarkUrl);
        }

        #endregion

        /// <summary>
        /// [OAuth] 自身のユーザー情報を取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Entities.Account> GetAccountAsync()
        {
            var apiUrl = ApiBaseUrl + "/my";

            using (var client = MakeAuthorizedHttpClient())
            using (var response = await client.GetAsync(apiUrl))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var account = JsonConvert.DeserializeObject<Entities.Account>(json);
                    Account = account;
                    return account;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new HttpRequestException("has not been bookmarked yet");
                }
            }
            throw new HttpRequestException("connection error");
        }

        public async Task<TimelineResponse> GetTimelineAsync()
        {
            var apiUrl = $"http://n.hatena.ne.jp/timeline.json?reftime=-{DateTime.Now.Ticks},0";

            using (var client = MakeAuthorizedHttpClient())
            {
                var json = await client.GetStringAsync(apiUrl);
                return JsonConvert.DeserializeObject<TimelineResponse>(json);
            }
        }
    }
}
