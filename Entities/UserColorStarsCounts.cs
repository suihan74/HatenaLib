using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Entities
{
    class UserColorStarsCountResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public Dictionary<string, UserColorStarsCount> Result { get; set; }
    }

    public class UserColorStarsCount
    {
        [JsonProperty("red")]
        public long Red { get; set; }

        [JsonProperty("green")]
        public long Green { get; set; }

        [JsonProperty("blue")]
        public long Blue { get; set; }

        [JsonProperty("purple")]
        public long Purple { get; set; }
    }
}
