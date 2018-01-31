using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib.Entities
{
    public enum StarColor
    {
        Yellow,
        Red,
        Green,
        Blue
    }

    public class Star : UserBase
    {
        [JsonProperty("quote")]
        public string Quote { get; set; }

        [JsonProperty("count")]
        private long? _Count = null;

        [JsonProperty("color")]
        private string _Color = null;

        [JsonIgnore]
        public long Count { get => _Count ?? 1; set => _Count = value; }

        [JsonIgnore]
        public StarColor Color
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

    public class StarEntry
    {
        [JsonProperty("uri")]
        public string Url { get; set; }

        [JsonProperty("stars")]
        public Star[] Stars { get; set; }

        [JsonProperty("colored_stars")]
        public Star[] ColoredStars { get; set; }

        [JsonIgnore]
        public long TotalStars
        {
            get
            {
                return _TotalStars ?? (_TotalStars = (Stars?.Sum(s => s.Count) ?? 0) + (ColoredStars?.Sum(s => s.Count) ?? 0)).Value;
            }
        }

        [JsonIgnore]
        public long? _TotalStars;
    }

    public class StarEntries
    {
        [JsonProperty("entries")]
        public StarEntry[] Entries { get; set; }

        [JsonProperty("can_comment")]
        public bool CanComment { get; set; }
    }
}
