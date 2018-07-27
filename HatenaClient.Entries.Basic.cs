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
            if (string.IsNullOrEmpty(Auth.UserName))
            {
                throw new InvalidOperationException("failed to get favorites. user: " + Auth.UserName);
            }
            var url = $"{BaseUrl}/{Auth.UserName}/favorite.rss";
            return await GetEntriesImplAsync(url, GetCookieHeader());
        }

        /// <summary>
        /// [配信終了][Basic] マイホットエントリーを取得する
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<IEnumerable<Entities.EntriesListItem>> GetMyHotEntriesAsync()
        {
            if (string.IsNullOrEmpty(Auth.UserName))
            {
                throw new InvalidOperationException("failed to get hotentries. user: " + Auth.UserName);
            }
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
            if (string.IsNullOrEmpty(Auth.UserName))
            {
                throw new InvalidOperationException("failed to get tags. user: " + Auth.UserName);
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
            if (string.IsNullOrEmpty(Auth.UserName))
            {
                throw new InvalidOperationException("failed to get tags. user: " + Auth.UserName);
            }
            return GetUserTaggedEntriesAsync(Auth.UserName, tag, offset);
        }
    }
}
