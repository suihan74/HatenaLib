using HatenaLib.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HatenaLib
{
    public partial class HatenaClient
    {
        /// <summary>
        /// rkパラメータ
        /// </summary>
        internal string Rk { get; private set; }

        /// <summary>
        /// rksパラメータ
        /// </summary>
        internal string RksForBookmark { get; set; }
        internal string RksForStar { get; set; }

        /// <summary>
        /// rkmパラメータ
        /// </summary>
        internal string Rkm { get; set; }

        /// <summary>
        /// Basic認証してrkを取得する
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task GetRandomKeyAsync()
        {
            var url = "https://www.hatena.ne.jp/login";
            var client = MakeHttpClient();

            var data = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "name", Auth.UserName },
                { "password", Auth.Password },
            });

            using (var response = await client.PostAsync(url, data))
            {
                var regex = new Regex(@"rk=(?<rk>\S+);");
                Rk = response.Headers.GetValues("Set-Cookie")
                                    .Where(c => regex.IsMatch(c))
                                    .Select(c => regex.Match(c).Groups["rk"].Value)
                                    .FirstOrDefault();
            }

            if (Rk == null)
            {
                throw new HttpRequestException("failed to authorize");
            }

            // ユーザー情報を取得
            var userApiUrl = $"{BaseUrl}/my.name";
            client.DefaultRequestHeaders.Add("Cookie", $"rk={Rk}");
            using (var response = await client.GetAsync(userApiUrl))
            {
                var json = await response.Content.ReadAsStringAsync();
                var rks = JObject.Parse(json).Value<string>("rks");
                var rkm = JObject.Parse(json).Value<string>("rkm");

                RksForBookmark = rks;
                Rkm = rkm;
            }
        }

        /// <summary>
        /// 必要に応じてrkを取得する
        /// </summary>
        /// <returns></returns>
        private async Task CheckRk()
        {
            if (Rk == null)
            {
                try
                {
                    await GetRandomKeyAsync();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("randomkey has been not initialized", e);
                }
            }
        }

        /// <summary>
        /// rkを埋め込んだクッキーヘッダを作成
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetCookieHeader()
        {
            return new Dictionary<string, string>
            {
                { "Cookie", $"rk={Rk}" }
            };
        }

        /// <summary>
        /// rkを元にrksを取得
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetRksForStarAsync()
        {
            if (RksForStar != null) { return RksForStar; }

            await CheckRk();

            // 何故かentries.jsonに含まれて返ってくるので使う
            var apiUrl = $"{StarBaseUrl}/entries.json";
            var content = await GetAsync(apiUrl, GetCookieHeader());
            var str = await content.ReadAsStringAsync();
            var rks = JObject.Parse(str).Value<string>("rks");

            RksForStar = rks;
            return rks;
        }

        /// <summary>
        /// 通知を取得
        /// </summary>
        /// <returns></returns>
        public Task<NoticeResponse> GetNotifyAsync()
        {
            var apiUrl = $"https://www.hatena.ne.jp/notify/api/pull?{DateTime.Now.Ticks}";
            return GetJsonObjectAsync<NoticeResponse>(apiUrl, GetCookieHeader());
        }
    }
}
