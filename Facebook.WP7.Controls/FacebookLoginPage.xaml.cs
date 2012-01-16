using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Navigation;

namespace Facebook.WP7.Controls {
    public partial class FacebookLoginPage : IFacebookLoginPage {
        private const string RedirectUrl = "http://www.facebook.com/connect/login_success.html";
        private const string StateKeyValue = "FacebookLoginPageStateValue";

        public FacebookLoginPage() {
            InitializeComponent();
        }

        public string AccessToken { get; set; }

        public string AppId { get; set; }

        public string ExtendedPermissions { get; set; }

        public void ShowDialog() {
            if (!string.IsNullOrWhiteSpace(AccessToken)) {
                LoginStatusText.Visibility = Visibility.Visible;
                LoginProgressBar.Visibility = Visibility.Visible;

                var webRequest = WebRequest.CreateHttp("https://graph.facebook.com/me?access_token=" + AccessToken);
                webRequest.Method = "GET";
                webRequest.UserAgent = "Windows Phone 7";
                webRequest.BeginGetResponse(a => {
                    using (var httpWebResponse = (HttpWebResponse) webRequest.EndGetResponse(a)) {
                        bool success = httpWebResponse.StatusCode == HttpStatusCode.OK;
                        if (success) {
                            AccessToken = AccessToken;
                            CloseLoginPage();
                        }
                        else {
                            NavigateToLoginUrl();
                        }
                    }
                }, webRequest);
            }
            else {
                NavigateToLoginUrl();
            }
        }

        public void Logout() {
            LoadCompletedEventHandler loadCompleted = null;
            loadCompleted = (sender, e) => {
                if (facebookLoginBrowser.SaveToString().Contains("logout_form")) {
                    facebookLoginBrowser.InvokeScript("eval", "document.forms['logout_form'].submit();");
                    facebookLoginBrowser.Visibility = Visibility.Collapsed;
                    facebookLoginBrowser.LoadCompleted -= loadCompleted;
                    AccessToken = null;
                }
                facebookLoginBrowser.Navigated += FacebookLoginBrowserNavigated;
                CloseLoginPage();
            };

            LoginStatusText.Visibility = Visibility.Collapsed;
            facebookLoginBrowser.Navigated -= FacebookLoginBrowserNavigated;
            facebookLoginBrowser.LoadCompleted += loadCompleted;
            facebookLoginBrowser.Navigate(new Uri("https://www.facebook.com/logout.php"));
        }

        private void NavigateToLoginUrl() {
            var loginUrl = string.Format("http://www.facebook.com/dialog/oauth/?response_type=token&scope={0}&client_id={1}&redirect_uri={2}&display=touch", ExtendedPermissions, AppId, RedirectUrl);
            facebookLoginBrowser.Navigate(new Uri(loginUrl));
        }

        protected override void OnBackKeyPress(CancelEventArgs e) {
            e.Cancel = true;
            CloseLoginPage();
        }

        private void CloseLoginPage() {
            Dispatcher.BeginInvoke(() => NavigationService.GoBack());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            base.OnNavigatedFrom(e);

            if (e.Uri.ToString() == "app://external/")
                State[StateKeyValue] = AccessToken;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            if (State.ContainsKey(StateKeyValue)) {
                AccessToken = State[StateKeyValue] as string;

                if (NavigationService.CanGoBack) {
                    NavigationService.GoBack();
                }
            }
        }

        private void FacebookLoginBrowserNavigated(object sender, NavigationEventArgs e) {
            if (string.IsNullOrEmpty(e.Uri.Fragment)) {
                facebookLoginBrowser.Visibility = Visibility.Visible;
                LoginProgressBar.Visibility = Visibility.Collapsed;
                LoginStatusText.Visibility = Visibility.Collapsed;
                return;
            }

            if (e.Uri.AbsoluteUri.Replace(e.Uri.Fragment, "") == RedirectUrl) {
                string text = HttpUtility.HtmlDecode(e.Uri.Fragment).TrimStart('#');
                var pairs = text.Split('&');
                foreach (var pair in pairs) {
                    var keyValue = pair.Split('=');
                    if (keyValue.Length == 2) {
                        if (keyValue[0] == "access_token") {
                            AccessToken = keyValue[1];
                            CloseLoginPage();
                            return;
                        }
                    }
                }
            }
        }
    }
}