using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib.Entities
{
    public class Bookmark : UserBase
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("private")]
        public bool IsPrivate { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonIgnore]
        private string _TagsText;

        [JsonIgnore]
        public string TagsText
        {
            get
            {
                return _TagsText ?? (_TagsText = Tags?.Length > 0 ? string.Join(",", Tags) : string.Empty);
            }
        }

        [JsonIgnore]
        public StarEntry StarEntry { get; set; }

        [JsonIgnore]
        public DateTime Timestamp { get => _Timestamp == default(DateTime) ? _Created_DateTime : _Timestamp; }

        /// <summary>
        /// for posting or updating bookmarks
        /// </summary>
        [JsonProperty("created_datetime")]
        private DateTime _Created_DateTime { get; set; }

        /// <summary>
        /// for get entries
        /// </summary>
        [JsonProperty("timestamp")]
        private DateTime _Timestamp { get; set; }
    }
}
