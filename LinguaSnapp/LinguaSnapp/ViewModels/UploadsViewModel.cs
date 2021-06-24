using LinguaSnapp.Enums;
using LinguaSnapp.Interfaces;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using MDS.Essentials.Shared.Services;
using MDS.Essentials.Shared.Services.AlertDialog;
using MDS.Essentials.Shared.ViewExtensions.Spinner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace LinguaSnapp.ViewModels
{
    class UploadsViewModel : HomeViewModel, ISubmissionHandler
    {
        private List<GroupOfSubmissionCardViewModels> submissionList = new List<GroupOfSubmissionCardViewModels>();
        public List<GroupOfSubmissionCardViewModels> SubmissionList { get => submissionList; private set => SetProperty(ref submissionList, value); }

        private bool noneLabelVisible = true;
        public bool NoneLabelVisible { get => noneLabelVisible; set => SetProperty(ref noneLabelVisible, value); }

        public IconLabelButtonViewModel UploadAllButton { get; }

        private static bool isRefreshing;

        public UploadsViewModel()
        {
            // Set upload button
            UploadAllButton = new IconLabelButtonViewModel
            {
                IconBackgroundColour = (Color)Application.Current.Resources["Primary"],
                IconBorderColour = (Color)Application.Current.Resources["Primary"],
                ImageSource = ImageSource.FromFile("ic_upload_all_white"),
                LabelVisible = false,
                IconSize = 56,
                LabelText = (string)Application.Current.Resources["uploads_all"],
                LabelColour = (Color)Application.Current.Resources["Secondary"],
                TappedCommand = new Command(() =>
                {
                    // Ask whether they want to upload
                    DependencyService.Get<IAlertDialog>().ShowAlert(
                        (string)Application.Current.Resources["alert_upex_title"],
                        (string)Application.Current.Resources["alert_upload_all_body"],
                        negativeButton:
                        new AlertDialogButton
                        {
                            Text = (string)Application.Current.Resources["alert_cancel"]
                        },
                        positiveButton:
                        new AlertDialogButton
                        {
                            Text = (string)Application.Current.Resources["alert_upload_upload"],
                            Action = new Command(async () =>
                            {
                                // Get all groups of submissions which are eligible to be sent
                                var groups = SubmissionList.Where(g => g.Status != SubmissionStatus.Sent);

                                // Do nothing if no eligible submissions
                                if (groups.Count() < 1) return;

                                // Display spinner
                                await Spinner.ShowAsync((string)Application.Current.Resources["upload_spinner"]);

                                // Flag to be updated by the background task
                                bool allValid = true;

                                // Background task to do the processing
                                await Task.Run(async () =>
                                {
                                    for (int i = 0; i < groups.Count(); ++i)
                                    {
                                        // Get group
                                        var grp = groups.ElementAt(i);

                                        for (int j = 0; j < grp.Count(); ++j)
                                        {
                                            // Get submission view model
                                            var s = grp.ElementAt(j);

                                            // Update spinner subtitle
                                            Spinner.SetSubtitle($"{s.Title} ({j + 1} / {grp.Count()})");

                                            // Attempt to send
                                            var res = await DataService.Instance.UploadSubmissionAsync(s.SubId);

                                            // Handle result
                                            var msg = res.Result == UploadResult.UploadAttemptResult.ServerError ?
                                                    $"{res.DataServiceReply.Message} (Code {res.DataServiceReply.Code})" :
                                                    res.Message;
                                            if (res.Result != UploadResult.UploadAttemptResult.Success)
                                            {
                                                // If submission is invalid then set flag and continue
                                                if (res.Result == UploadResult.UploadAttemptResult.SubmissionInvalid)
                                                {
                                                    allValid = false;
                                                    continue;
                                                }
                                                else
                                                {
                                                    Device.BeginInvokeOnMainThread(() =>
                                                    {
                                                        DependencyService.Get<IAlertDialog>().ShowAlert(
                                                            (string)Application.Current.Resources["alert_error"],
                                                            msg,
                                                            new AlertDialogButton
                                                            {
                                                                Text = (string)Application.Current.Resources["alert_ok"]
                                                            }
                                                        );
                                                    });

                                                    // Stop sending if there is a server error
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                })

                                // Hide spinner when complete
                                .ContinueWith(async task =>
                                {
                                    // Hide spinner
                                    await Spinner.HideAsync();

                                    // Present popup if necessary
                                    if (!allValid)
                                    {
                                        // Present popup to user
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            DependencyService.Get<IAlertDialog>().ShowAlert(
                                                (string)Application.Current.Resources["alert_upload_all_invalid_title"],
                                                (string)Application.Current.Resources["alert_upload_all_invalid_body"],
                                                new AlertDialogButton
                                                {
                                                    Text = (string)Application.Current.Resources["alert_ok"]
                                                }
                                            );
                                        });
                                    }

                                    // Refresh submissions
                                    await RefreshSubmissionsAsync();
                                });
                            })
                        },
                        neutralButton: new AlertDialogButton
                        {
                            Text = (string)Application.Current.Resources["alert_upload_export"],
                            Action = new Command(async () =>
                            {
                                // If no submissions then return
                                if (SubmissionList == null || SubmissionList.Count() == 0)
                                    return;

                                // Display toast
                                MainThread.BeginInvokeOnMainThread(async () =>
                                {
                                    // Crashes on iOS 12.4
                                    try
                                    {
                                        await Application.Current.MainPage.DisplayToastAsync(new ToastOptions
                                        {
                                            BackgroundColor = Color.White,
                                            MessageOptions = new MessageOptions
                                            {
                                                Foreground = (Color)Application.Current.Resources["PrimaryLightBackground"],
                                                Message = (string)Application.Current.Resources["upload_spinner_export"],
                                                Padding = (double)Application.Current.Resources["margin"]
                                            }
                                        });
                                    }
                                    catch { }
                                });

                                // Create PDF document
                                var pdfWriter = new PdfWriter();

                                // Loop over submissions
                                for (int i = 0; i < SubmissionList.Count(); ++i)
                                {
                                    // Get group
                                    var grp = SubmissionList.ElementAt(i);

                                    for (int j = 0; j < grp.Count(); ++j)
                                    {
                                        // Get submission view model
                                        var s = grp.ElementAt(j);

                                        // Start editing new submission
                                        await SubmissionService.Instance.StartEditingAsync(s.SubId, false);

                                        // Get contents
                                        var meta = SubmissionService.Instance.GetSubmissionMeta();
                                        var descriptors = SubmissionService.Instance.GetDescriptors();
                                        var comments = SubmissionService.Instance.GetComments();
                                        var translations = SubmissionService.Instance.GetTranslations();
                                        var oneLang = SubmissionService.Instance.GetOneLanguageDominant();

                                        // Construct document
                                        pdfWriter.WriteSubmission(SubmissionService.Instance.GetSubmissionStatus(), meta, descriptors, comments, translations, oneLang);
                                    }
                                }

                                // Save and share
                                string filename = $"Export_{DateTime.UtcNow.ToString("yyyy-MM-ddTHH.mm.ss")}.pdf";
                                var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filename);
                                pdfWriter.SaveAndClose(file);
                                Console.WriteLine($"Sharing {filename}...");

                                // Construct share request
                                var req = new ShareFileRequest
                                {
                                    Title = (string)Application.Current.Resources["upload_share_title"],
                                    File = new ShareFile(file)
                                };

                                // If it is an iPad then provide rectangle for popover
                                if (Device.Idiom == TargetIdiom.Tablet && Device.RuntimePlatform == Device.iOS)
                                {
                                    req.PresentationSourceBounds = new System.Drawing.Rectangle
                                    {
                                        Y = (int)(Application.Current.MainPage.Height),
                                        Width = (int)Application.Current.MainPage.Width,
                                        Height = 1
                                    };
                                };

                                // Share
                                await Share.RequestAsync(req);
                            })
                        });
                })
            };
        }

        public async Task RefreshSubmissionsAsync()
        {
            // Exit if already refreshing
            if (isRefreshing) return;

            // Set flag
            isRefreshing = true;

            // Get submissions from the DB
            Debug.WriteLine("Requesting view models...");
            SubmissionList = await SubmissionService.Instance.GetSubmissionsAsGroupsOfViewModelsAsync();

            // Set the visibility
            NoneLabelVisible = SubmissionList.Count() == 0;

            // Unset flag
            isRefreshing = false;
            Debug.WriteLine("Refresh complete");
        }
    }
}
