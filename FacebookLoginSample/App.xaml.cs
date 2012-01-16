using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace FacebookLoginSample {
    public partial class App {
        public App() {
            UnhandledException += ApplicationUnhandledException;
            InitializeComponent();
            InitializePhoneApplication();
        }

        public PhoneApplicationFrame RootFrame { get; private set; }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e) {
            if (Debugger.IsAttached) {
                Debugger.Break();
            }
        }

        private void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e) {
            if (Debugger.IsAttached) {
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized;

        // Do not add any additional code to this method
        private void InitializePhoneApplication() {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e) {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}