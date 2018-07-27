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
        private static readonly Dictionary<string, HttpClient> HttpClientPool = new Dictionary<string, HttpClient>();
        private static HttpClient Client;

        public static HttpClient MakeHttpClient(string url)
        {
            if (Client == null)
            {
                Client = new HttpClient();
                Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            }

            // コネクションを定期的に更新してDNS変更を反映する
            try
            {
                var uri = new Uri(url);
                var sp = ServicePointManager.FindServicePoint(uri);
                sp.ConnectionLeaseTimeout = 60 * 1000;
            }
            catch (Exception)
            {}

            return Client;
        }

        #region GET

        protected static Task<HttpContent> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var client = MakeHttpClient(url);
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    if (headers != null)
                    {
                        foreach (var pair in headers)
                        {
                            req.Headers.TryAddWithoutValidation(pair.Key, pair.Value);
                        }
                    }

                    var response = await client.SendAsync(req);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return response.Content;
                    }
                }
                catch (Exception e)
                {
                    throw new HttpRequestException("GET request failed. URL: " + url, e);
                }

                throw new HttpRequestException("GET request failed. URL: " + url);
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
                var client = MakeHttpClient(url);
                var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new FormUrlEncodedContent(contents)
                };
                if (headers != null)
                {
                    foreach (var pair in headers)
                    {
                        req.Headers.TryAddWithoutValidation(pair.Key, pair.Value);
                    }
                }

                var response = await client.SendAsync(req);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("failed to POST. StatusCode: " + response.StatusCode);
                }

                return response.Content;
            });
        }

        #endregion

        #region REQUEST

        protected static Task<HttpContent> SendAsync(HttpRequestMessage req, Dictionary<string ,string> headers = null)
        {
            return Task.Run(async () =>
            {
                var client = MakeHttpClient(req.RequestUri.AbsoluteUri);
                if (headers != null)
                {
                    foreach (var pair in headers)
                    {
                        req.Headers.TryAddWithoutValidation(pair.Key, pair.Value);
                    }
                }

                var response = await client.SendAsync(req);
                return response.Content;
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
