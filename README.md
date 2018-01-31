# HatenaLib

This is a .NET Standard 2.0 library to access Hatena API for C# (7.0 or later).

This library contains AsyncOAuth (https://github.com/neuecc/AsyncOAuth).


## First step

~~~C#
var app = new HatenaLib.ApplicationIdentity
{
    ConsumerKey = hogehoge,
    ConsumerSecret = fugafuga,
};
var client = await HatenaLib.OAuthClient.AuthorizeAsync(app, username, password);
~~~
