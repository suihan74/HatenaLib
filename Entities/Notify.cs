using HatenaLib.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Entities
{
    public class NotifyObject
    {
        [JsonProperty("color")]
        private string _Color = null;

        [JsonIgnore]
        public StarColor StarColor
        {
            get
            {
                switch (_Color)
                {
                    case "red": return StarColor.Red;
                    case "green": return StarColor.Green;
                    case "blue": return StarColor.Blue;
                    default: return StarColor.Yellow;
                }
            }
        }

        [JsonProperty("user")]
        public string UserName { get; set; }
    }

    public class NotifyMetadata
    {
        [JsonProperty("subject_title")]
        public string SubjectTitle { get; set; }
    }

    public enum NotifyType
    {
        Unknown,

        Bookmark,
        Favorited,
        IdCall,
        Star,
    }

    public class Notify
    {
        [JsonProperty("created")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("metadata")]
        public NotifyMetadata Metadata { get; set; }

        [JsonProperty("modified")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime ModifiedAt { get; set; }

        [JsonProperty("object")]
        public NotifyObject[] Objects { get; set; }

        [JsonProperty("verb")]
        private string _Verb = null;

        [JsonIgnore]
        public NotifyType Verb
        {
            get
            {
                switch (_Verb)
                {
                    case "bookmark": return NotifyType.Bookmark;
                    case "idcall": return NotifyType.IdCall;
                    case "star": return NotifyType.Star;
                    default: return NotifyType.Unknown;
                }
            }
        }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }

    public class NotifyResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("last_seen")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastSeen { get; set; }

        [JsonProperty("notices")]
        public Notify[] notices { get; set; }
    }
}
