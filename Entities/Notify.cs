using HatenaLib.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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


        [JsonIgnore]
        private string _UserImageUrl = null;

        [JsonIgnore]
        public string UserImageUrl
        {
            get => _UserImageUrl ?? (_UserImageUrl = Objects?.LastOrDefault()?.UserImageUrl ?? string.Empty);
        }

        [JsonIgnore]
        private string _Message = null;

        [JsonIgnore]
        public string Message
        {
            get
            {
                if (_Message != null)
                {
                    return _Message;
                }

                var users = Objects
                    .Reverse()
                    .DistinctBy(n => n.UserName)
                    .ToArray();
                var tops = users.Take(3);
                var last = tops.Last();

                var msg = new StringBuilder();
                foreach (var u in tops)
                {
                    msg.Append(u.UserName);
                    msg.Append("さん");
                    if (u != last)
                    {
                        msg.Append(", ");
                    }
                }

                var total = users.Count();
                if (total > 3)
                {
                    msg.Append($", ほか{total - 3}人");
                }

                switch (Verb)
                {
                    case NoticeType.Bookmark:
                        msg.Append("が, あなたのエントリーをブックマークしました");
                        break;

                    case NoticeType.IdCall:
                        msg.Append("から, IDコールがありました");
                        break;

                    case NoticeType.Star:
                        msg.Append("が, あなたのブックマークにスターをつけました");
                        break;
                }

                return (_Message = msg.ToString());
            }
        }
    }

    public class NoticeResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("last_seen")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastSeen { get; set; }

        [JsonProperty("notices")]
        public Notice[] Notices { get; set; }
    }
}
