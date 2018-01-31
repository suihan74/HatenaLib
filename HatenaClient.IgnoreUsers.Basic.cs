using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib
{
    class IgnoreUsers
    {
        [JsonProperty("ignore_users")]
        public Dictionary<string, string>[] Users { get; set; }
    }

    public partial class HatenaClient
    {
        /// <summary>
        /// [Basic] 無視ユーザーリストを取得する
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetIgnoreUsersAsync()
        {
            await CheckRk();
            var url = $"{BaseUrl}/{Auth.UserName}/ignore.json?{DateTime.Now.Ticks}";
            var response = await GetJsonObjectAsync<IgnoreUsers>(url, GetCookieHeader());
            var result = response.Users.Select(x => x.Where(p => p.Key == "name").Select(p => p.Value).First());

            foreach (var u in result)
            {
                IgnoreUsersSet.Add(u);
            }

            return result;
        }

        /// <summary>
        /// [Basic] 対象ユーザーを無視ユーザーリストに追加する
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task IgnoreUserAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("username is empty", nameof(userName));
            }

            await CheckRk();
            var url = $"{BaseUrl}/{Auth.UserName}/api.ignore";
            var data = new Dictionary<string, string>
            {
                {"rks", RksForBookmark},
                {"username", userName}
            };
            await PostAsync(url, data, GetCookieHeader());
            IgnoreUsersSet.Add(userName);
        }

        /// <summary>
        /// [Basic] 対象ユーザーを無視ユーザーリストから除外する
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task UnignoreUserAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("username is empty", nameof(userName));
            }

            await CheckRk();
            var url = $"{BaseUrl}/{Auth.UserName}/api.unignore";
            var data = new Dictionary<string, string>
            {
                {"rks", RksForBookmark},
                {"username", userName}
            };
            await PostAsync(url, data, GetCookieHeader());
            IgnoreUsersSet.Remove(userName);
        }
    }
}
