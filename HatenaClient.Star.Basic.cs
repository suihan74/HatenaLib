﻿using HatenaLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib
{
    public partial class HatenaClient
    {
        /// <summary>
        /// [Basic] はてなスターを付ける
        /// </summary>
        /// <param name="url">対象URL</param>
        /// <param name="quote">引用文</param>
        /// <param name="color">カラースター</param>
        /// <returns></returns>
        public async Task AddStarAsync(string url, string quote = null, Entities.StarColor color = default(Entities.StarColor))
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("url is empty", nameof(url));
            }

            await CheckRk();
            var apiUrl = $"{StarBaseUrl}/star.add.json";

            var rks = await GetRksForStarAsync();
            var data = new Dictionary<string, string>
            {
                {"rks", rks},
                {"uri", url},
                {"quote", quote ?? string.Empty},
            };
            if (color != default(Entities.StarColor))
            {
                data.Add("color", color.ToString("G").ToLower());
            }

            await PostAsync(apiUrl, data, GetCookieHeader());
        }

        /// <summary>
        /// [Basic] 最近ほかのユーザーから自分にスターが付けられたエントリーを取得する
        /// </summary>
        /// <returns></returns>
        public async Task<StarEntry[]> GetStarEntriesReceivedAsync()
        {
            var apiUrl = $"{StarBaseUrl}/{Account.Name}/report.json?" + DateTime.Now.Ticks;

            await CheckRk();
            var res = await GetJsonObjectAsync<StarEntries>(apiUrl, GetCookieHeader());
            return res.Entries;
        }

        /// <summary>
        /// [Basic] 最近自分がスターを付けたエントリーを取得する
        /// </summary>
        /// <returns></returns>
        public async Task<StarEntry[]> GetStarEntriesSentAsync()
        {
            var apiUrl = $"{StarBaseUrl}/{Account.Name}/stars.json?" + DateTime.Now.Ticks;

            await CheckRk();
            return await GetJsonObjectAsync<StarEntry[]>(apiUrl, GetCookieHeader());
        }

    }
}
