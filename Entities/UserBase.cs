using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatenaLib.Entities
{
    public class UserBase
    {
        [JsonIgnore]
        public string UserName
        {
            get => _User ?? _Name;
            set
            {
                _User = value;
                _UserImageUrl = null;
            }
        }

        [JsonProperty("user")]
        private string _User { get; set; }

        [JsonProperty("name")]
        private string _Name { get; set; }

        [JsonIgnore]
        private string _UserImageUrl;

        [JsonIgnore]
        public string UserImageUrl
        {
            get => _UserImageUrl ?? (_UserImageUrl = UserNameToImageUrl(UserName));
        }

        /// <summary>
        /// for JsonConvert
        /// </summary>
        public UserBase() { }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public UserBase(UserBase src)
        {
            _User = src._User;
            _Name = src._Name;
            _UserImageUrl = src._UserImageUrl;
        }

        /// <summary>
        /// ユーザー名からアイコンURLを取得する
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string UserNameToImageUrl(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return string.Empty;
            }
            else
            {
                return $"http://cdn1.www.st-hatena.com/users/{(userName.Substring(0, 2))}/{userName}/profile.gif";
            }
        }
    }
}
