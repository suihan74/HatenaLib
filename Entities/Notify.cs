using HatenaLib.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Entities
{
    public class NoticeObject : UserBase
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
    }

    public class NotifyMetadata
    {
        [JsonProperty("subject_title")]
        public string SubjectTitle { get; set; }
    }

    public enum NoticeType
    {
        Unknown,

        Bookmark,
        Favorited,
        IdCall,
        Star,
    }

    public class Notice
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
        public NoticeObject[] Objects { get; set; }

        [JsonProperty("verb")]
        private string _Verb = null;

        [JsonIgnore]
        public NoticeType Verb
        {
            get
            {
                switch (_Verb)
                {
                    case "bookmark": return NoticeType.Bookmark;
                    case "idcall": return NoticeType.IdCall;
                    case "star": return NoticeType.Star;
                    default: return NoticeType.Unknown;
                }
            }
        }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }

    public class NoticeResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("last_seen")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastSeen { get; set; }

        [JsonProperty("notices")]
        public Notice[] notices { get; set; }
    }
}
