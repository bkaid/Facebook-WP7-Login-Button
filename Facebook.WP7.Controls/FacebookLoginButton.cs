using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Facebook.WP7.Controls {
    public class FacebookLoginButton : Button {
        private const string ButtonPartName = "FacebookLoginButton";

        public static readonly DependencyProperty AccessTokenProperty = DependencyProperty.Register("AccessToken", typeof (string), typeof (FacebookLoginButton), new PropertyMetadata(null, null));
        public static readonly DependencyProperty AppIdProperty = DependencyProperty.Register("AppId", typeof (string), typeof (FacebookLoginButton), new PropertyMetadata(null, null));
        public static readonly DependencyProperty ExtendedPermissionsProperty = DependencyProperty.Register("ExtendedPermissions", typeof (string), typeof (FacebookLoginButton), new PropertyMetadata(null, null));

        private static readonly Uri LoginPageUri = new Uri("/Facebook.WP7.Controls;component/FacebookLoginPage.xaml", UriKind.Relative);
        private static readonly Uri LogoutPageUri = new Uri("/Facebook.WP7.Controls;component/FacebookLoginPage.xaml?action=logout", UriKind.Relative);

        private ButtonBase _button;
        private IFacebookLoginPage _facebookLoginPage;
        private PhoneApplicationFrame _frame;
        private object _frameContentWhenOpened;
        private bool _hasLoadedDialog;

        public FacebookLoginButton() {
            DefaultStyleKey = typeof (FacebookLoginButton);
            Loaded += FacebookLoginButtonLoaded;
        }

        public string AccessToken {
            get { return (string) GetValue(AccessTokenProperty); }
            set {
                SetValue(AccessTokenProperty, value);
                if (_hasLoadedDialog && !string.IsNullOrWhiteSpace(value)) {
                    OnAccessTokenChanged(new AccessTokenChangedEventArgs(value));
                }
            }
        }

        public string AppId {
            get { return (string) GetValue(AppIdProperty); }
            set { SetValue(AppIdProperty, value); }
        }

        public string ExtendedPermissions {
            get { return (string) GetValue(ExtendedPermissionsProperty); }
            set { SetValue(ExtendedPermissionsProperty, value); }
        }

        private void FacebookLoginButtonLoaded(object sender, RoutedEventArgs e) {
            Loaded -= FacebookLoginButtonLoaded;
            Click += OnLoginButtonClick;
        }

        public event EventHandler<AccessTokenChangedEventArgs> AccessTokenChanged;

        protected virtual void OnAccessTokenChanged(AccessTokenChangedEventArgs e) {
            var handler = AccessTokenChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        public override void OnApplyTemplate() {
            if (_button != null) {
                _button.Click -= OnLoginButtonClick;
            }

            base.OnApplyTemplate();

            _button = GetTemplateChild(ButtonPartName) as ButtonBase;
            if (_button != null) {
                _button.Click += OnLoginButtonClick;
            }
        }

        private void OnLoginButtonClick(object sender, RoutedEventArgs e) {
            AuthenticateUser();
        }

        public void AuthenticateUser() {
            _hasLoadedDialog = true;
            OpenPage(LoginPageUri);
        }

        public void Logout() {
            OpenPage(LogoutPageUri);
        }

        private void OpenPage(Uri uri) {
            if (_frame == null) {
                _frame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (_frame != null) {
                    _frameContentWhenOpened = _frame.Content;
                    _frame.Navigated += OnFrameNavigated;
                    _frame.NavigationStopped += OnFrameNavigationStoppedOrFailed;
                    _frame.NavigationFailed += OnFrameNavigationStoppedOrFailed;
                    _frame.Navigate(uri);
                }
            }
        }

        private void CloseLoginPage() {
            if (_frame != null) {
                _frame.Navigated -= OnFrameNavigated;
                _frame.NavigationStopped -= OnFrameNavigationStoppedOrFailed;
                _frame.NavigationFailed -= OnFrameNavigationStoppedOrFailed;
                _frame = null;
                _frameContentWhenOpened = null;
            }
            if (_facebookLoginPage != null) {
                AccessToken = _facebookLoginPage.AccessToken;
                _facebookLoginPage = null;
            }
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e) {
            if (e.Content == _frameContentWhenOpened) {
                CloseLoginPage();
            }
            else if (_facebookLoginPage == null) {
                _facebookLoginPage = e.Content as IFacebookLoginPage;
                if (_facebookLoginPage != null) {
                    _facebookLoginPage.AppId = AppId;
                    _facebookLoginPage.ExtendedPermissions = ExtendedPermissions;
                    _facebookLoginPage.AccessToken = AccessToken;

                    if (e.Uri.ToString().Contains("action=logout")) {
                        _facebookLoginPage.Logout();
                    }
                    else {
                        _facebookLoginPage.ShowDialog();
                    }
                }
            }
        }

        private void OnFrameNavigationStoppedOrFailed(object sender, EventArgs e) {
            CloseLoginPage();
        }
    }
}