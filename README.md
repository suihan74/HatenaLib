# HatenaLib

This is a .NET Standard 2.0 library to access Hatena API for C# (7.0 or later). It supports both APIs that require Basic authentication and APIs that require OAuth.

This library contains AsyncOAuth (https://github.com/neuecc/AsyncOAuth).


## First step

~~~C#
var app = new HatenaLib.ApplicationIdentity(
    consumerKey: hogehoge,
    consumerSecret: fugafuga);
var client = await HatenaLib.OAuthClient.AuthorizeAsync(app, username, password);
~~~
