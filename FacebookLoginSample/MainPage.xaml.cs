using System.Windows;
using Facebook.WP7.Controls;

namespace FacebookLoginSample {
    public partial class MainPage {
        public MainPage() {
            InitializeComponent();
        }

        private void AccessTokenChanged(object sender, AccessTokenChangedEventArgs e) {
            MessageBox.Show("Access token: " + e.AccessToken);
        }

        private void Logout(object sender, RoutedEventArgs e) {
            FacebookButton.Logout();
        }
    }
}