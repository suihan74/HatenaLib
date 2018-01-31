using AsyncOAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HatenaLib
{
    /// <summary>
    /// コンシューマキー/シークレット
    /// </summary>
    public class ApplicationIdentity
    {
        public string ConsumerKey;
        public string ConsumerSecret;
    }

    /// <summary>
    /// OAuth認証を行いHatenaClientを取得するためのクラス
    /// </summary>
    public static class OAuthClient
    {
        private static readonly string RequestTokenUrl = "https://www.hatena.com/oauth/initiate";
        private static readonly string AuthorizeBaseUrl = "https://www.hatena.ne.jp/oauth/authorize";
        private static readonly string AccessTokenUrl = "https://www.hatena.com/oauth/token";

        private static readonly string CallbackUrl = "http://localhost/";

        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0";

        private static bool Initialized = false;

        public static void Initialize()
        {
            if (!Initialized)
            {
                OAuthUtility.ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };
                Initialized = true;
            }
        }

        /// <summary>
        /// ブローカーを開いてOAuth認証する
        /// </summary>
        /// <returns></returns>
        public static async Task<HatenaClient> AuthorizeAsync(ApplicationIdentity app, string username, string password)
        {
            Initialize();
            try
            {
                var authorizer = new OAuthAuthorizer(app.ConsumerKey, app.ConsumerSecret);
                var requestToken = await GetRequestTokenAsync(authorizer);
                var accessToken = await GetAccessTokenAsync(authorizer, requestToken, username, password);
                var auth = new Entities.Auth(accessToken.Key, accessToken.Secret, username, password);
                return new HatenaClient(auth);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("authentication failure", e);
            }
        }

        /// <summary>
        /// リクエストトークン取得
        /// </summary>
        /// <param name="authorizer"></param>
        /// <returns></returns>
        private static async Task<RequestToken> GetRequestTokenAsync(OAuthAuthorizer authorizer)
        {
            var requestTokenResponse = await authorizer.GetRequestToken(
                RequestTokenUrl,
                new[]
                {
                    new KeyValuePair<string, string>("oauth_callback", CallbackUrl)
                },
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("scope", "read_private,read_public,write_public")
                }));
            return requestTokenResponse.Token;
        }

        /// <summary>
        /// rkパラメータを取得
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static async Task<string> GetRkAsync(string username, string password)
        {
            var url = "https://www.hatena.ne.jp/login";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            var data = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "name", username },
                { "password", password },
            });

            string rk = string.Empty;
            using (var response = await client.PostAsync(url, data))
            {
                var regex = new Regex(@"rk=(?<rk>\S+);");
                rk = response.Headers.GetValues("Set-Cookie")
                             .Where(c => regex.IsMatch(c))
                             .Select(c => regex.Match(c).Groups["rk"].Value)
                             .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(rk))
            {
                throw new HttpRequestException("rk");
            }

            return rk;
        }

        /// <summary>
        /// アクセストークン取得
        /// </summary>
        /// <param name="authorizer"></param>
        /// <param name="requestToken"></param>
        /// <returns></returns>
        private static async Task<AccessToken> GetAccessTokenAsync(OAuthAuthorizer authorizer, RequestToken requestToken, string username, string password)
        {
            var requestUri = new Uri(authorizer.BuildAuthorizeUrl(AuthorizeBaseUrl, requestToken));

            // rkmの取得にrkが必要
            var rk = await GetRkAsync(username, password);

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            client.DefaultRequestHeaders.Add("Cookie", $"rk={rk}");

            // 認証の自動化にrkmが必要
            string rks = string.Empty;
            string rkm = string.Empty;
            using (var response = await client.GetAsync("http://b.hatena.ne.jp/my.name"))
            {
                var json = await response.Content.ReadAsStringAsync();
                rks = JObject.Parse(json).Value<string>("rks");
                rkm = JObject.Parse(json).Value<string>("rkm");
            }

            if (string.IsNullOrEmpty(rkm))
            {
                throw new HttpRequestException("rkm");
            }

            var param = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"rkm", rkm},
                {"oauth_token", requestToken.Key},
                {"name", "%E8%A8%B1%E5%8F%AF%E3%81%99%E3%82%8B"}
            });

            // OAuth認証
            using (var response = await client.PostAsync(AuthorizeBaseUrl, param))
            {
                var responseData = response.Headers.Location;
                var regex = new Regex(@"oauth_verifier=(.+)&?");
                var match = regex.Match(responseData.Query);
                if (match.Success)
                {
                    var verifier = Uri.UnescapeDataString(match.Groups[1].Value);
                    var accessTokenResponse = await authorizer.GetAccessToken(AccessTokenUrl, requestToken, verifier);
                    return accessTokenResponse.Token;
                }
                else
                {
                    throw new HttpRequestException("failed to get the access token");
                }
            }
        }
    }
}
