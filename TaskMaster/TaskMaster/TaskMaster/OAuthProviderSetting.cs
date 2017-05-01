﻿using System;
using Xamarin.Auth;

namespace TaskMaster
{
    public class OAuthProviderSetting
    {
        public OAuth2Authenticator LoginWithProvider(string provider)
        {
            OAuth2Authenticator auth = null;
            switch (provider)
            {
                case "Google":
                {
                    auth = new OAuth2Authenticator(                        
                        "723494873981-qsnnp5vsa72f4d74bo8m8kqfsrbo25cq.apps.googleusercontent.com",
                        null,
                        "https://www.googleapis.com/auth/userinfo.email",
                        new Uri("https://accounts.google.com/o/oauth2/auth"),
                        new Uri("com.xamarin.traditional.standard.samples.oauth.providers.android:/oauth2redirect"),
                        new Uri("https://www.googleapis.com/oauth2/v4/token"),
                        isUsingNativeUI: true
                    );
                    break;
                }
                case "FaceBook":
                {
                    auth = new OAuth2Authenticator(
                        "647866935403018",
                        "email",
                        new Uri("https://www.facebook.com/v2.9/dialog/oauth?client_id=647866935403018&redirect_uri=http://www.facebook.com/connect/login_success.html"),
                        new Uri("http://www.facebook.com/connect/login_success.html"),
                        isUsingNativeUI: true
                    );
                    break;
                }
            }
            return auth;
        }
    }
}

