using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HatenaLib
{
    public class BaseHttpClient
    {
        protected static HttpClient MakeHttpClient(Dictionary<string, string> headers = null)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    client.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                }
            }
            return client;
        }

        #region GET

        protected static Task<HttpContent> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            return Task.Run(async () =>
            {
                using (var client = MakeHttpClient(headers))
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content;
                    }
                    else
                    {
                        throw new HttpRequestException();
                    }
                }
            });
        }

        /// <summary>
        /// Jsonから変換したオブジェクトを返す
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected static async Task<T> GetJsonObjectAsync<T>(string url, Dictionary<string, string> headers = null)
        {
            var result = await GetAsync(url, headers);
            var content = await result.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// XmlReaderを返す
        /// <para>IDisposable</para>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected static async Task<XDocument> GetXDocumentAsync(string url, Dictionary<string, string> headers = null)
        {
            var result = await GetAsync(url, headers);
            using (var content = await result.ReadAsStreamAsync())
            {
                return XDocument.Load(content);
            }
        }

        #endregion

        #region POST

        protected static Task<HttpContent> PostAsync(string url, Dictionary<string, string> contents, Dictionary<string, string> headers = null)
        {
            return Task.Run(async () =>
            {
                using (var client = MakeHttpClient(headers))
                {
                    var response = await client.PostAsync(url, new FormUrlEncodedContent(contents));
                    return response.Content;
                }
            });
        }

        #endregion

        #region REQUEST

        protected static Task<HttpContent> SendAsync(HttpRequestMessage req, Dictionary<string ,string> headers = null)
        {
            return Task.Run(async () =>
            {
                using (var client = MakeHttpClient(headers))
                {
                    var response = await client.SendAsync(req);
                    return response.Content;
                }
            });
        }

        /// <summary>
        /// Jsonから変換したオブジェクトを返す
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected static async Task<T> RequestJsonObjectAsync<T>(HttpRequestMessage req, Dictionary<string, string> headers = null)
        {
            var result = await SendAsync(req, headers);
            var content = await result.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        #endregion
    }
}
