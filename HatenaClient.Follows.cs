using HatenaLib.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib
{
    public partial class HatenaClient
    {
        /// <summary>
        /// お気に入りユーザーを取得
        /// </summary>
        /// <returns></returns>
        public static Task<UserBase[]> GetFollowsAsync(string userName)
        {

            var apiUrl = $"{BaseUrl}/{userName}/follow.json?{DateTime.Now.Ticks}";
            return GetJsonObjectAsync<UserBase[]>(apiUrl);
        }

        /// <summary>
        /// 自分のお気に入りユーザーを取得
        /// </summary>
        /// <returns></returns>
        public Task<UserBase[]> GetFollowsAsync() => GetFollowsAsync(Account.Name);

        /// <summary>
        /// [Basic] ユーザーをフォローする
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task FollowAsync(string userName)
        {
            await CheckRk();
            var apiUrl = $"{BaseUrl}/{userName}/follow.follow";
            await GetAsync(apiUrl, GetCookieHeader());
        }

        /// <summary>
        /// [Basic] ユーザーをフォロー解除する
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task UnfollowAsync(string userName)
        {
            await CheckRk();
            var apiUrl = $"{BaseUrl}/{userName}/follow.unfollow";
            await GetAsync(apiUrl, GetCookieHeader());
        }

        /// <summary>
        /// お気に入られユーザーを取得
        /// </summary>
        public static Task<UserBase[]> GetFollowedAsync(string userName)
        {
            var apiUrl = $"{BaseUrl}/{userName}/followed.json?{DateTime.Now.Ticks}";
            return GetJsonObjectAsync<UserBase[]>(apiUrl);
        }

        /// <summary>
        /// 自分のお気に入られユーザーを取得
        /// </summary>
        /// <returns></returns>
        public Task<UserBase[]> GetFollowedAsync() => GetFollowedAsync(Account.Name);
    }
}
