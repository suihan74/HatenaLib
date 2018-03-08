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
            set => _User = value;
        }

        [JsonProperty("user")]
        private string _User { get; set; }

        [JsonProperty("name")]
        private string _Name { get; set; }

        [JsonIgnore]
        public string UserImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(UserName))
                {
                    return string.Empty;
                }
                else
                {
                    return $"http://cdn1.www.st-hatena.com/users/{(UserName.Substring(0, 2))}/{UserName}/profile.gif";
                }
            }
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
        }
    }
}
