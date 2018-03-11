using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib
{
    public partial class HatenaClient
    {
        /// <summary>
        /// [Basic] お気に入りユーザーのブクマ一覧を取得する
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Entities.EntriesListItem>> GetFavoriteUsersEntriesAsync()
        {
            var url = $"{BaseUrl}/{Auth.UserName}/favorite.rss";
            return await GetEntriesImplAsync(url, GetCookieHeader());
        }

        /// <summary>
        /// [Basic] マイホットエントリーを取得する
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Entities.EntriesListItem>> GetMyHotEntriesAsync()
        {
            await CheckRk();
            var url = $"{BaseUrl}/{Auth.UserName}/hotentry.rss";
            return await GetEntriesImplAsync(url, GetCookieHeader());
        }

        /// <summary>
        /// [Basic] タグ一覧を取得する
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Task<IEnumerable<Entities.Tag>> GetUserTagsAsync()
        {
            if (string.IsNullOrEmpty(Auth?.UserName))
            {
                throw new InvalidOperationException("failed to get tags");
            }
            return GetUserTagsAsync(Auth.UserName);
        }

        /// <summary>
        /// [Basic] 特定タグが付けられたユーザーフィードを取得する
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Task<IEnumerable<Entities.EntriesListItem>> GetUserTaggedEntriesAsync(string tag, long? offset = null)
        {
            if (string.IsNullOrEmpty(Auth?.UserName))
            {
                throw new InvalidOperationException("failed to get tags");
            }
            return GetUserTaggedEntriesAsync(Auth.UserName, tag, offset);
        }
    }
}
