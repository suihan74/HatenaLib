using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Entities
{
    public class TargetObject
    {
        [JsonProperty("url_name")]
        public string UrlName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
    }
}
