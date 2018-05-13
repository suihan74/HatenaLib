using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Entities
{
    /// <summary>
    /// ユーザー認証情報
    /// </summary>
    public struct Auth
    {
        public string Token { get; }
        public string TokenSecret { get; }
        public string UserName { get; }
        public string Password { get; }

        public ApplicationIdentity App { get; }

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
