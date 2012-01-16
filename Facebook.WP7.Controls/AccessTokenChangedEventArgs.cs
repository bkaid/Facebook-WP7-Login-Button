using System;

namespace Facebook.WP7.Controls {
    public class AccessTokenChangedEventArgs : EventArgs {
        public AccessTokenChangedEventArgs(string accessToken) {
            AccessToken = accessToken;
        }

        public string AccessToken { get; private set; }
    }
}