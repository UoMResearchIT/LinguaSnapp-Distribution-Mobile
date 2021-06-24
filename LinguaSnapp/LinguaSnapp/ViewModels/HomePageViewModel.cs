using LinguaSnapp.Interfaces;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using MDS.Essentials.Shared.Services;
using MDS.Essentials.Shared.Services.AlertDialog;
using MDS.Essentials.Shared.ViewExtensions.Spinner;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels
{
    class HomePageViewModel : HomeViewModel, IHomeViewModel
    {
        public int NumberUploaded { get; private set; }

        private FormattedString welcomeText;
        public FormattedString WelcomeText { get => welcomeText; set => SetProperty(ref welcomeText, value); }

        public ICommand TakePhotoCommand { get; }

        public IconLabelButtonViewModel TakePhotoButtonViewModel { get; }

        public HomePageViewModel()
        {
            UpdateWelcomeText();

            TakePhotoButtonViewModel = new IconLabelButtonViewModel
            {
                IconSize = 256,
                ImageSource = "ic_icon_primary",
                LabelSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                LabelText = (string)Application.Current.Resources["welcome_take_photo"],
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"],
                TappedCommand = new Command(async () =>
                {
                    // Ask for location permission here as blocks if done later
                    try
                    {
                        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                        Debug.WriteLine($"Location Permission = {status}");

                        // Launch picker
                        var photo = await MediaPicker.CapturePhotoAsync();

                        // If file received
                        if (photo != null)
                        {
                            // Convert to a byte array
                            var stream = await photo.OpenReadAsync();
                            var bytes = new byte[stream.Length];
                            var bytesRead = await stream.ReadAsync(bytes, 0, (int)stream.Length);

                            // Get GPS
                            await Spinner.ShowAsync((string)Application.Current.Resources["welcome_gps_spinner"]);

                            // Check permission
                            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                            Debug.WriteLine($"Location Permission = {status}");

                            // Launch background task to get GPS
                            GpsQueryResult res = null;
                            await Task.Run(async () =>
                            {
                                // Get GPS
                                res = await GetCurrentLocation(status);

                                // Create a submission model
                                Spinner.SetTitle((string)Application.Current.Resources["welcome_create_spinner"]);
                                var id = await SubmissionService.Instance.CreateNewSubmissionAsync(
                                bytes, res.Latitude, res.Longitude);

                                // Post UI changes to main thread
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    // Hide spinner
                                    await Spinner.HideAsync();

                                    // Trigger change of shell
                                    await (Application.Current as IShell)?.LoadEditorShellAsync(id);

                                    // If failed then present message
                                    if (res.Message != null)
                                    {
                                        DependencyService.Get<IAlertDialog>().ShowAlert(
                                            (string)Application.Current.Resources["alert_error"],
                                            res.Message,
                                            new AlertDialogButton
                                            {
                                                Text = (string)Application.Current.Resources["alert_ok"]
                                            });
                                    }
                                });
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"{e.Message}");
                    }
                })
            };
        }

        public void UpdateWelcomeText()
        {
            // Get the uploaded total from preferences
            NumberUploaded = Preferences.Get((string)Application.Current.Resources["key_num_sent"], 0);

            // Create string
            WelcomeText = new FormattedString
            {
                Spans =
                {
                    new Span { Text = (string)Application.Current.Resources["welcome_1"] },
                    new Span { Text = $" {NumberUploaded} " },
                    new Span { Text = (string)Application.Current.Resources["welcome_2"] }
                }
            };
        }

        // Async task to get GPS
        async Task<GpsQueryResult> GetCurrentLocation(PermissionStatus status)
        {
            Location location = null;
            string msg = null;
            try
            {
                if (status == PermissionStatus.Granted)
                {
                    Debug.WriteLine("Getting location...");
                    var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                    location = await Geolocation.GetLocationAsync(request);
                    Debug.WriteLine("Location get returned");
                }
                else
                {
                    Debug.WriteLine("Permission Denied");
                    throw new PermissionException($"Permission Denied: {status}");
                }

                // If no location obtained then fail gracefully
                if (location == null)
                {
                    Debug.WriteLine("Location failed!");
                    throw new Exception("Location failed!");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Crashes.TrackError(fnsEx);
                msg = $"{fnsEx.Message} {(string)Application.Current.Resources["alert_no_gps_support"]}";
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Crashes.TrackError(fneEx);
                msg = $"{fneEx.Message} {(string)Application.Current.Resources["alert_no_gps_disabled"]}";
            }
            catch (PermissionException pEx)
            {
                Crashes.TrackError(pEx);
                msg = $"{pEx.Message} {(string)Application.Current.Resources["alert_no_gps_permission"]}";
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                msg = $"{ex.Message} {(string)Application.Current.Resources["alert_no_gps_other"]}";
            }
            finally
            {
                Debug.WriteLine($"Leaving location with message: {msg}");
            }

            // Return
            return new GpsQueryResult(location?.Latitude ?? 999, location?.Longitude ?? 999, msg);
        }
    }

    class GpsQueryResult
    {
        internal double Latitude { get; }

        internal double Longitude { get; }

        internal string Message { get; }

        internal GpsQueryResult(double lati, double longi, string msg)
        {
            Latitude = lati;
            Longitude = longi;
            Message = msg == null ? msg : string.Join("\n", msg, (string)Application.Current.Resources["alert_enter_location"]);
        }
    }
}
