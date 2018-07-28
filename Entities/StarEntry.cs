using HatenaLib.Utilities;
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
        Blue,
        Purple,
    }

    public class Star : UserBase
    {
        [JsonProperty("quote")]
        public string Quote { get; set; }

        [JsonProperty("count")]
        private long? _Count = null;

        [JsonProperty("color")]
        [JsonConverter(typeof(StarColorConverter))]
        public StarColor Color { get; set; }

        [JsonIgnore]
        public long Count { get => _Count ?? 1; set => _Count = value; }

        /// <summary>
        /// for JsonConvert
        /// </summary>
        public Star() { }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public Star(Star src)
            : base(src)
        {
            Quote = src.Quote;
            _Count = src._Count;
            Color = src.Color;
        }
    }

    public class ColoredStarEntry
    {
        [JsonProperty("stars")]
        public Star[] Stars { get; set; }

        [JsonProperty("color")]
        [JsonConverter(typeof(StarColorConverter))]
        public StarColor Color { get; set; }
    }

    public class StarEntry
    {
        [JsonProperty("uri")]
        public string Url { get; set; }

        [JsonProperty("stars")]
        public Star[] Stars { get; set; }

        [JsonProperty("colored_stars")]
        private ColoredStarEntry[] _ColoredStarsOrigin { get; set; }

        [JsonIgnore]
        private Star[] _ColoredStars = null;

        [JsonIgnore]
        public Star[] ColoredStars
        {
            get
            {
                if (_ColoredStars == null)
                {
                    _ColoredStars = _ColoredStarsOrigin?
                        .Select(e =>
                        {
                            foreach (var star in e.Stars) star.Color = e.Color;
                            return e.Stars;
                        })
                        .Aggregate<IEnumerable<Star>>((s, x) => s.Concat(x))
                        .ToArray()
                        ?? new Star[0];
                }
                return _ColoredStars;
            }
        }

        [JsonIgnore]
        public long TotalStars
        {
            get
            {
                return _TotalStars ?? (_TotalStars = (Stars?.Sum(s => s.Count) ?? 0) + (ColoredStars?.Sum(s => s.Count) ?? 0)).Value;
            }
        }

        [JsonIgnore]
        private long? _TotalStars;
    }

    public class StarEntries
    {
        [JsonProperty("entries")]
        public StarEntry[] Entries { get; set; }

        [JsonProperty("can_comment")]
        public bool CanComment { get; set; }
    }
}
