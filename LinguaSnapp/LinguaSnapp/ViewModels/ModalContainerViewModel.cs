using LinguaSnapp.Interfaces;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using LinguaSnapp.Views;
using LinguaSnapp.Views.Popups;
using MDS.Essentials.Shared.Services;
using MDS.Essentials.Shared.Services.AlertDialog;
using MDS.Essentials.Shared.ViewExtensions.Spinner;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels
{
    class ModalContainerViewModel : BaseViewModel, IModalContainerViewModel
    {
        public ICommand RegisterLinkTapped { get; }

        public ICommand LoginLinkTapped { get; }

        public ICommand RegisterButtonTapped { get; }

        public ICommand LoginButtonTapped { get; }

        public ICommand MailingListTapped { get; }

        public IconLabelButtonViewModel AccessButtonViewModel { get; }

        public IconLabelButtonViewModel DataButtonViewModel { get; }

        public IconLabelButtonViewModel AboutButtonViewModel { get; }

        public EntryEditorWithIconViewModel UserCodeViewModel { get; }

        public EntryEditorWithIconViewModel PasswordViewModel { get; }

        public EntryEditorWithIconViewModel ConfirmPasswordViewModel { get; }

        public EntryEditorWithIconViewModel EmailViewModel { get; }

        public DynamicHeaderViewModel HeaderViewModel { get; } = new DynamicHeaderViewModel();

        private bool joinMailingList;
        public bool JoinMailingList { get => joinMailingList; set => SetProperty(ref joinMailingList, value); }

        public ModalContainerViewModel()
        {
            UserCodeViewModel = new EntryEditorWithIconViewModel
            {
                IconSource = "ic_user_primary",
                Placeholder = (string)Application.Current.Resources["login_usercode"]
            };

            PasswordViewModel = new EntryEditorWithIconViewModel
            {
                IconSource = "ic_lock_primary",
                Placeholder = (string)Application.Current.Resources["login_password"],
                IsPassword = true
            };

            ConfirmPasswordViewModel = new EntryEditorWithIconViewModel
            {
                IconSource = "ic_lock_tick_primary",
                Placeholder = (string)Application.Current.Resources["register_confirm"],
                IsPassword = true
            };

            EmailViewModel = new EntryEditorWithIconViewModel
            {
                IconSource = "ic_email_primary",
                Placeholder = (string)Application.Current.Resources["register_email"]
            };

            RegisterLinkTapped = new Command(() =>
            {
                MessagingCenter.Send<IModalContainerViewModel>(this, (string)Application.Current.Resources["msg_register_tapped"]);
            });

            LoginLinkTapped = new Command(() =>
            {
                MessagingCenter.Send<IModalContainerViewModel>(this, (string)Application.Current.Resources["msg_login_tapped"]);
            });

            MailingListTapped = new Command(() => JoinMailingList = !JoinMailingList);

            AboutButtonViewModel = new IconLabelButtonViewModel
            {
                IconSize = 162,
                ImageSource = "ic_icon_primary",
                LabelSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                LabelText = (string)Application.Current.Resources["info_about"],
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"],
                TappedCommand = new Command(async () => await Shell.Current.Navigation.PushPopupAsync(new InfoContentPopup("Resources.about.html")))
            };

            AccessButtonViewModel = new IconLabelButtonViewModel
            {
                ImageSource = "ic_access_primary",
                LabelSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                LabelText = (string)Application.Current.Resources["info_access"],
                TappedCommand = new Command(async () => await Shell.Current.Navigation.PushPopupAsync(new InfoContentPopup("Resources.accessibility.html"))),
                IconSize = 128,
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"]
            };

            DataButtonViewModel = new IconLabelButtonViewModel
            {
                ImageSource = "ic_data_primary",
                LabelSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                LabelText = (string)Application.Current.Resources["info_data"],
                TappedCommand = new Command(async () => await Shell.Current.Navigation.PushPopupAsync(new InfoContentPopup("Resources.dataprotection.html"))),
                IconSize = 128,
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"]
            };

            LoginButtonTapped = new Command(async () =>
            {
                // Validation
                if (!CheckValidity(false)) return;

                // Show the spinner
                await Spinner.ShowAsync((string)Application.Current.Resources["login_spinner"]);

                // Fire off the login method
                await Task.Run(async () => await DataService.Instance.LoginAsync(
                    UserCodeViewModel.EntryText.Trim(),
                    PasswordViewModel.EntryText.Trim()
                    )
                ).ContinueWith(reply =>
                {
                    var res = reply.Result;
                    if (res.Success)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.Navigation.PopModalAsync();
                            await Spinner.HideAsync();
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DependencyService.Get<IAlertDialog>().ShowAlert(
                                (string)Application.Current.Resources["alert_error"],
                                $"Code {res.Code}: {res.Message}",
                                negativeButton:
                                new AlertDialogButton
                                {
                                    Text = (string)Application.Current.Resources["alert_cancel"],
                                    Action = new Command(async () => await Spinner.HideAsync())
                                },
                                positiveButton:
                                new AlertDialogButton
                                {
                                    Text = (string)Application.Current.Resources["alert_retry"],
                                    Action = new Command(async () =>
                                    {
                                        await Spinner.HideAsync();
                                        await Task.Delay(250);
                                        LoginButtonTapped.Execute(null);
                                    })
                                });
                        });
                    }
                });
            });

            RegisterButtonTapped = new Command(async () =>
            {
                // Validation
                if (!CheckValidity(true)) return;

                // Show the spinner
                await Spinner.ShowAsync((string)Application.Current.Resources["register_spinner"]);

                // Fire off the registration method
                await Task.Run(async () => await DataService.Instance.RegisterAsync(
                    UserCodeViewModel.EntryText.Trim(),
                    PasswordViewModel.EntryText.Trim(),
                    EmailViewModel.EntryText?.Trim(),
                    JoinMailingList
                    )
                ).ContinueWith(async reply =>
                {
                    var res = reply.Result;
                    await Spinner.HideAsync();
                    if (res.Success)
                    {
                        Device.BeginInvokeOnMainThread(async () => await Shell.Current.Navigation.PopModalAsync());
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DependencyService.Get<IAlertDialog>().ShowAlert(
                                (string)Application.Current.Resources["alert_error"],
                                $"Code {res.Code}: {res.Message}",
                                negativeButton:
                                new AlertDialogButton
                                {
                                    Text = (string)Application.Current.Resources["alert_cancel"]
                                },
                                positiveButton:
                                new AlertDialogButton
                                {
                                    Text = (string)Application.Current.Resources["alert_retry"],
                                    Action = new Command(() => RegisterButtonTapped.Execute(null))
                                }
                            );
                        });
                    }
                });
            });
        }

        // Check the username and password fields
        private bool CheckValidity(bool register)
        {
            // Set icons
            UserCodeViewModel.StatusIconSource = null;
            PasswordViewModel.StatusIconSource = null;
            ConfirmPasswordViewModel.StatusIconSource = null;
            EmailViewModel.StatusIconSource = null;

            // Init flags
            bool statusUser = false;
            bool statusPass = false;
            bool statusConfirm = false;
            bool statusEmail = false;
            string msg = string.Empty;

            // Username
            if (string.IsNullOrEmpty(msg))
            {
                statusUser = true;
                if (string.IsNullOrWhiteSpace(UserCodeViewModel.EntryText))
                {
                    msg = (string)Application.Current.Resources["alert_username_blank"];
                    statusUser = false;
                }
                else if (UserCodeViewModel.EntryText.Trim().Length < 5)
                {
                    msg = (string)Application.Current.Resources["alert_username_short"];
                    statusUser = false;
                }
                UserCodeViewModel.StatusIconSource = ImageSource.FromFile(statusUser ? "ic_correct_green" : "ic_wrong_red");
            }

            // Password
            if (string.IsNullOrEmpty(msg))
            {
                statusPass = true;
                if (string.IsNullOrWhiteSpace(PasswordViewModel.EntryText))
                {
                    msg = (string)Application.Current.Resources["alert_password_blank"];
                    statusPass = false;
                }
                else if (PasswordViewModel.EntryText.Trim().Length < 5)
                {
                    msg = (string)Application.Current.Resources["alert_password_short"];
                    statusPass = false;
                }
                PasswordViewModel.StatusIconSource = ImageSource.FromFile(statusPass ? "ic_correct_green" : "ic_wrong_red");
            }

            // Confirm
            if (string.IsNullOrEmpty(msg) && register)
            {
                statusConfirm = true;
                if (string.IsNullOrWhiteSpace(ConfirmPasswordViewModel.EntryText))
                {
                    msg = (string)Application.Current.Resources["alert_password_blank"];
                    statusConfirm = false;
                }
                else if (PasswordViewModel.EntryText.Trim() != ConfirmPasswordViewModel.EntryText.Trim())
                {
                    msg = (string)Application.Current.Resources["alert_password_mismatch"];
                    statusConfirm = false;
                }
                ConfirmPasswordViewModel.StatusIconSource = ImageSource.FromFile(statusConfirm ? "ic_correct_green" : "ic_wrong_red");
            }

            // Email
            if (string.IsNullOrEmpty(msg) && !string.IsNullOrWhiteSpace(EmailViewModel.EntryText) && register)
            {
                statusEmail = true;
                if (!IsValidEmail(EmailViewModel.EntryText.Trim()))
                {
                    msg = (string)Application.Current.Resources["alert_email_invalid"];
                    statusEmail = false;
                }
                EmailViewModel.StatusIconSource = ImageSource.FromFile(statusEmail ? "ic_correct_green" : "ic_wrong_red");
            }

            // Present alert
            if (!string.IsNullOrEmpty(msg))
            {
                DependencyService.Get<IAlertDialog>().ShowAlert(
                    (string)Application.Current.Resources["alert_error"],
                    msg,
                    negativeButton:
                    new AlertDialogButton
                    {
                        Text = (string)Application.Current.Resources["alert_ok"]
                    }
                );
            }
            return string.IsNullOrEmpty(msg);
        }

        // Check an email address
        private bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Update shared backing field properties
        public void Update(ModalContainer.PageLayoutType pageLayout)
        {
            UserCodeViewModel.Placeholder = pageLayout == ModalContainer.PageLayoutType.Register ? (string)Application.Current.Resources["register_usercode"] : (string)Application.Current.Resources["login_usercode"];
            PasswordViewModel.Placeholder = pageLayout == ModalContainer.PageLayoutType.Register ? (string)Application.Current.Resources["register_password"] : (string)Application.Current.Resources["login_password"];

            // Reset status icons
            UserCodeViewModel.StatusIconSource = null;
            PasswordViewModel.StatusIconSource = null;
            ConfirmPasswordViewModel.StatusIconSource = null;
        }
    }
}
