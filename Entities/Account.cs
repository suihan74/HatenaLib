using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib.Entities
{
    public class Account
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string ProfileImageUrl
        {
            get => $"http://cdn1.www.st-hatena.com/users/{(Name.Substring(0, 2))}/{Name}/profile.gif";
        }

        [JsonProperty("plususer")]
        public bool IsPlusUser { get; set; }

        [JsonProperty("private")]
        public bool IsPrivate { get; set; }

        [JsonProperty("is_oauth_twitter")]
        public bool? IsActivatedTwitter { get; set; }

        [JsonProperty("is_oauth_evernote")]
        public bool? IsActivatedEvernote { get; set; }

        [JsonProperty("is_oauth_facebook")]
        public bool? IsActivatedFacebook { get; set; }

        [JsonProperty("is_oauth_mixi_check")]
        public bool? IsActivatedMixi { get; set; }

        /// <summary>
        /// 無視ユーザーリストを表す正規表現
        /// </summary>
        [JsonProperty("ignores_regex")]
        public string IgnoresRegex { get; set; }

        [JsonProperty("rks")]
        public string RandomKeySignature { get; set; }

        [JsonProperty("rkm")]
        public string RandomKeyMd5 { get; set; }
    }
}
