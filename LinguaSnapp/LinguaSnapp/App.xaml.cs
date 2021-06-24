using LinguaSnapp.Interfaces;
using LinguaSnapp.Services;
using LinguaSnapp.Views;
using MDS.Essentials.Shared.Services;
using MDS.Essentials.Shared.ViewExtensions.Spinner;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LinguaSnapp
{
    public partial class App : Application, IShell
    {
        private string homeRouteBeforeEditorLoad;
        internal static double DeviceDensity { get; private set; }

        public App()
        {
            // MDS Essentials
            MDS.Essentials.Shared.SharedApp.Init();

            // Inflate dictionary
            InitializeComponent();

            // Initialise PDF writer
            DependencyService.Get<IPdfWriter>().Init();
        }

        protected override async void OnStart()
        {
            // Load primary shell
            await LoadHomeShellAsync();

            // Set spinner appearance (if used)
            Spinner.SetTextColour(
                Application.Current.RequestedTheme == OSAppTheme.Dark && Device.RuntimePlatform == Device.iOS ?
                Color.White :
                (Color)Application.Current.Resources["PrimaryLightBackground"]);
            Spinner.SetIconSource(ImageSource.FromFile("ic_icon_primary"));
            Spinner.SetIconTint(
                Application.Current.RequestedTheme == OSAppTheme.Dark && Device.RuntimePlatform == Device.iOS ?
                Color.White :
                (Color)Application.Current.Resources["PrimaryLightBackground"]);

            // Set dialog appearance
            DependencyService.Get<IAlertDialog>().SetTintColour(
                Application.Current.RequestedTheme == OSAppTheme.Dark && Device.RuntimePlatform == Device.iOS ?
                Color.White :
                (Color)Application.Current.Resources["PrimaryLightBackground"]);

            // Get screen density here for reference from background threads
            DeviceDensity = DeviceDisplay.MainDisplayInfo.Density;

            // Load configuration database
            await Task.Run(async () => await ConfigurationService.Instance.InitialiseConfigurationDatabaseAsync())
                .ContinueWith(async task =>
            {
                if (!await DataService.Instance.IsLoggedInAsync())
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // Display the login/register modal
                        await Shell.Current.Navigation.PushModalAsync(new ModalContainer());
                    });
                }
            });
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public async Task LoadEditorShellAsync(int pk)
        {
            homeRouteBeforeEditorLoad = Shell.Current?.CurrentItem?.CurrentItem?.Route;
            await SubmissionService.Instance.StartEditingAsync(pk);
            MainPage = new AppShellEditor();
        }

        public async Task LoadHomeShellAsync(string route = null)
        {
            MainPage = new AppShellHome();
            if (route == null)
            {
                if (homeRouteBeforeEditorLoad != null) await RouteToAsync(homeRouteBeforeEditorLoad);
            }
            else await RouteToAsync(route);
        }

        private async Task RouteToAsync(string route)
        {
            await Shell.Current.GoToAsync("//" + route);
        }
    }
}
