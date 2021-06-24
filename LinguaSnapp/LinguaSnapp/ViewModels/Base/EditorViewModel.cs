using LinguaSnapp.Enums;
using LinguaSnapp.Interfaces;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.ContentViews;
using MDS.Essentials.Shared.Services;
using MDS.Essentials.Shared.Services.AlertDialog;
using MDS.Essentials.Shared.ViewExtensions.Spinner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.Base
{
    abstract class EditorViewModel : ControlViewModel
    {
        public IconLabelButtonViewModel CancelButtonViewModel { get; }

        public IconLabelButtonViewModel SaveButtonViewModel { get; }

        public IconLabelButtonViewModel UploadButtonViewModel { get; }

        public IconLabelButtonViewModel DeleteButtonViewModel { get; }

        private ImageSource previewImage;
        public ImageSource PreviewImage { get => previewImage; private set => SetProperty(ref previewImage, value); }

        // Fields
        private static int numInstances = 0;

        public EditorViewModel()
        {
            // Set readonly flag
            var subStatus = SubmissionService.Instance.GetSubmissionStatus();
            IsReadOnly = subStatus != SubmissionStatus.Draft;

            CancelButtonViewModel = new IconLabelButtonViewModel
            {
                ImageSource = "ic_cancel_white",
                LabelColour = (Color)Application.Current.Resources["Secondary"],
                LabelText = (string)Application.Current.Resources["editor_cancel"],
                TappedCommand = new Command(async () =>
                {
                    await SubmissionService.Instance.StopEditingAsync();
                    await (Application.Current as IShell)?.LoadHomeShellAsync();
                }),
                IconBackgroundColour = (Color)Application.Current.Resources["Primary"],
                IconBorderColour = (Color)Application.Current.Resources["Primary"],
                IconSize = 84,
                HasShadow = false
            };

            SaveButtonViewModel = new IconLabelButtonViewModel
            {
                ImageSource = "ic_save_white",
                LabelColour = (Color)Application.Current.Resources["Secondary"],
                LabelText = (string)Application.Current.Resources["editor_save"],
                TappedCommand = new Command(async () =>
                {
                    await SaveAndExitAsync();
                }),
                IconVisible = !IsReadOnly,
                IconBackgroundColour = (Color)Application.Current.Resources["Primary"],
                IconBorderColour = (Color)Application.Current.Resources["Primary"],
                IconSize = 84,
                HasShadow = false
            };

            UploadButtonViewModel = new IconLabelButtonViewModel
            {
                ImageSource = "ic_upload_all_white",
                LabelColour = (Color)Application.Current.Resources["Secondary"],
                LabelText = (string)Application.Current.Resources["editor_upload"],
                TappedCommand = new Command(async () =>
                {
                    // Check if touched all sections
                    if (!SubmissionService.Instance.IsTouchedSubmission())
                    {
                        DependencyService.Get<IAlertDialog>().ShowAlert(
                            (string)Application.Current.Resources["alert_really"],
                            (string)Application.Current.Resources["alert_not_touched"],
                            negativeButton: new AlertDialogButton { Text = (string)Application.Current.Resources["alert_no"] },
                            positiveButton: new AlertDialogButton
                            {
                                Text = (string)Application.Current.Resources["alert_yes"],
                                Action = new Command(async () => await SaveAndExitAsync(true))
                            }
                        );
                    }

                    // If all touched then proceed
                    else
                    {
                        await SaveAndExitAsync(true);
                    }
                }),
                IconVisible = subStatus != SubmissionStatus.Sent,
                IconBackgroundColour = (Color)Application.Current.Resources["Primary"],
                IconBorderColour = (Color)Application.Current.Resources["Primary"],
                IconSize = 84,
                HasShadow = false
            };

            DeleteButtonViewModel = new IconLabelButtonViewModel
            {
                ImageSource = "ic_delete_white",
                LabelColour = (Color)Application.Current.Resources["Secondary"],
                LabelText = (string)Application.Current.Resources["editor_delete"],
                TappedCommand = new Command(() =>
                {
                    DependencyService.Get<IAlertDialog>().ShowAlert(
                        (string)Application.Current.Resources["alert_really"],
                        (string)Application.Current.Resources["alert_delete_body"],
                        negativeButton:
                        new AlertDialogButton
                        {
                            Text = (string)Application.Current.Resources["alert_no"]
                        },
                        positiveButton:
                        new AlertDialogButton
                        {
                            IsDestructive = true,
                            Text = (string)Application.Current.Resources["alert_delete"],
                            Action = new Command(async () =>
                            {
                                await SubmissionService.Instance.StopEditingAsync(delete: true);
                                await (Application.Current as IShell)?.LoadHomeShellAsync();
                            })
                        });
                }),
                IconBackgroundColour = (Color)Application.Current.Resources["Primary"],
                IconBorderColour = (Color)Application.Current.Resources["Primary"],
                IconSize = 84,
                HasShadow = false
            };

            // Subscribe to messages so all sub-class view models write to the active submission
            MessagingCenter.Subscribe<EditorViewModel, CountdownEvent>(
                this,
                (string)Application.Current.Resources["msg_update_submission_from_vm"],
                (me, counter) =>
                {
                    UpdateSubmissionFromViewModel();
                    counter.Signal();
                });

            // Increment counter of instances after subscription
            Interlocked.Increment(ref numInstances);
        }

        // Explicit destructor to manage counter
        ~EditorViewModel()
        {
            Interlocked.Decrement(ref numInstances);
        }

        // Method to perform synchronised save and exit
        private async Task SaveAndExitAsync(bool shouldUpload = false)
        {
            // Don't block UI thread
            await Task.Run(async () =>
            {
                // Create counter to sychronise saving across derived classes
                Debug.WriteLine("Creating counter");
                var counter = new CountdownEvent(numInstances);
                MessagingCenter.Send(
                    this,
                    (string)Application.Current.Resources["msg_update_submission_from_vm"],
                    counter
                    );

                // Block
                counter.Wait();
                Debug.WriteLine("Save complete");

                // Set appropriate status
                SubmissionStatus status = SubmissionStatus.Draft;
                bool isValid = true;
                if (shouldUpload)
                {
                    isValid = SubmissionService.Instance.IsValidSubmission();
                    if (isValid) status = SubmissionStatus.Outbox;
                }
                Debug.WriteLine($"Submission is valid = {isValid}");

                // Respond to validity check
                if (!isValid)
                {
                    // Present a warning that we can't upload
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        DependencyService.Get<IAlertDialog>().ShowAlert(
                            (string)Application.Current.Resources["alert_upload_invalid"],
                            (string)Application.Current.Resources["alert_upload_invalid_body"],
                            new AlertDialogButton { Text = (string)Application.Current.Resources["alert_ok"] }
                        ));
                }
                else
                {
                    // Get ID of the submission before it is unloaded
                    var id = SubmissionService.Instance.GetSubmissionId();

                    // Unload submission so it writes to database
                    await SubmissionService.Instance.StopEditingAsync(status);

                    // If we don't need to upload then simply change shell
                    if (!shouldUpload)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await (Application.Current as IShell)?.LoadHomeShellAsync("route_uploads");
                        });
                    }

                    // Otherwise, upload
                    else
                    {
                        // Display spinner
                        await Spinner.ShowAsync((string)Application.Current.Resources["upload_spinner"]);

                        // Continue task by sending
                        bool success = false;
                        string msg = string.Empty;

                        // Trigger upload of the submission
                        var res = await DataService.Instance.UploadSubmissionAsync(id);

                        // Handle result
                        msg = res.Result == UploadResult.UploadAttemptResult.ServerError ?
                            $"{res.DataServiceReply.Message} (Code {res.DataServiceReply.Code})" :
                            res.Message;
                        success = res.Result == UploadResult.UploadAttemptResult.Success;

                        // Change shell
                        Device.BeginInvokeOnMainThread(async () => await (Application.Current as IShell)?.LoadHomeShellAsync("route_uploads"));

                        // Present error if necessary
                        Debug.WriteLine($"Upload returned: {success}");
                        if (!success)
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                DependencyService.Get<IAlertDialog>().ShowAlert(
                                    (string)Application.Current.Resources["alert_error"],
                                    msg,
                                    new AlertDialogButton
                                    {
                                        Text = (string)Application.Current.Resources["alert_ok"],
                                        Action = new Command(async () => await Spinner.HideAsync())
                                    });
                            });
                        }

                        // Dismiss spinner
                        else await Spinner.HideAsync();
                    }
                }
            });
        }

        // To be overridden by each VM for them to load relevant submission details from the active submission
        protected virtual void PopulateViewModelFromSubmission()
        {
            PreviewImage = SubmissionService.Instance.PreviewImage;
        }

        // To be overridden by each VM for them to write relevant submission details back to the acive submission
        protected virtual void UpdateSubmissionFromViewModel()
        {

        }
    }
}
