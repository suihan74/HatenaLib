using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;

namespace HatenaLib.Entities
{
    public class Tag
    {
        [JsonIgnore]
        public string Text { get; set; }

        [JsonProperty("index")]
        public long Index { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("timestamp")]
        public long TimeStamp { get; set; }
    }

    class TagsResponse
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("status")]
        public HttpStatusCode Status { get; set; }

        [JsonProperty("tags")]
        public Dictionary<string, Tag> Tags { get; set; }
    }
}
