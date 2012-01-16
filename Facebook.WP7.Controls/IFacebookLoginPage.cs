namespace Facebook.WP7.Controls {
    public interface IFacebookLoginPage {
        string AccessToken { get; set; }
        string ExtendedPermissions { get; set; }
        string AppId { get; set; }
        void ShowDialog();
        void Logout();
    }
}