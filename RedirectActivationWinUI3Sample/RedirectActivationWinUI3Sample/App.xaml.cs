using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RedirectActivationWinUI3Sample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var modernArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            var main = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");
            if (!main.IsCurrent)
            {
                await main.RedirectActivationToAsync(modernArgs);
                Exit();
                return;
            }

            main.Activated += Main_Activated;

            MainWindow = new MainWindow();
            MainWindow.Activate();
        }


        [DllImport("Microsoft.Internal.FrameworkUdk.dll", EntryPoint = "Windowing_GetWindowHandleFromWindowId", CharSet = CharSet.Unicode)]

        public static extern IntPtr GetWindowIdFromWindowHandle(IntPtr hwnd, out WindowId result);

        private void Main_Activated(object sender, Microsoft.Windows.AppLifecycle.AppActivationArguments e)
        {
            MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                // Update the UI (button) to show it was redirected
                MainWindow.UpdateAsRedirected();

                // Try to bring the window to foreground (currently doesn't work due to foreground rights not being there)
                GetWindowIdFromWindowHandle(WinRT.Interop.WindowNative.GetWindowHandle(MainWindow), out WindowId myWindowId);
                var appWindow = AppWindow.GetFromWindowId(myWindowId);
                appWindow.Show(true);
            });
        }

        public MainWindow MainWindow { get; private set; }
    }
}
