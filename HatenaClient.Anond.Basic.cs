using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HatenaLib
{
    public partial class HatenaClient
    {
        private string RkForAnond;
        private string RkmForAnond;
        private string SiForAnond;

        /// <summary>
        /// [Basic] 匿名ダイアリーにログイン
        /// </summary>
        /// <returns></returns>
        private async Task LoginAnonymousDiaryAsync()
        {
            try
            {
                var rk = string.Empty;
                var rkm = string.Empty;
                var si = string.Empty;

                var loginUrl = "https://hatelabo.jp/login";
                var userData = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"mode", "enter"},
                    {"key", Auth.UserName},
                    {"password", Auth.Password}
                });
                var client = MakeHttpClient();
                using (var response = await client.PostAsync(loginUrl, userData))
                {
                    var rkRegex = new Regex(@"rk=(?<rk>\S+);");
                    var siRegex = new Regex(@"si=(?<si>\S+);");
                    rk = response.Headers.GetValues("Set-Cookie")
                                         .Where(c => rkRegex.IsMatch(c))
                                         .Select(c => rkRegex.Match(c).Groups["rk"].Value)
                                         .FirstOrDefault();
                    si = response.Headers.GetValues("Set-Cookie")
                                         .Where(c => siRegex.IsMatch(c))
                                         .Select(c => siRegex.Match(c).Groups["si"].Value)
                                         .FirstOrDefault();
                    var md5 = MD5.Create();
                    var db = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(rk));
                    rkm = Convert.ToBase64String(db).Replace("==", "");
                }

                RkForAnond = rk;
                RkmForAnond = rkm;
                SiForAnond = si;
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        /// <summary>
        /// [Basic] 匿名ダイアリーに投稿する
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task PostAnonymousDiaryAsync(string title, string text)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("text is empty", nameof(text));
            }

            if (string.IsNullOrEmpty(RkForAnond))
            {
                await LoginAnonymousDiaryAsync();
            }

            var apiUrl = $"https://anond.hatelabo.jp/{Account.Name}/edit";
            var data = new Dictionary<string, string>
            {
                {"rkm", RkmForAnond},
                {"mode", "confirm"},
                {"id", ""},
                {"title", title},
                {"body", text},
                {"edit", "この内容を登録する"}
            };

            var header = new Dictionary<string, string>
            {
                { "Cookie", $"rk={RkForAnond};si={SiForAnond}" },
            };

            var res = await PostAsync(apiUrl, data, header);
        }
    }
}
