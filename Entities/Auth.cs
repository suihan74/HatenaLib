using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Entities
{
    public class Auth
    {
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ApplicationIdentity App { get; set; }

        // for JsonConvert
        public Auth()
        {
        }

        public Auth(ApplicationIdentity app, string token, string tokenSecret, string username, string password)
        {
            App = app;
            Token = token;
            TokenSecret = tokenSecret;
            UserName = username;
            Password = password;
        }
    }
}
