using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using MDS.Essentials.Shared.Services;
using MDS.Essentials.Shared.Services.AlertDialog;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.Popups
{
    class AddTranslationPopupViewModel : BaseViewModel
    {
        public EntryEditorWithIconViewModel OtherLanguageViewModel { get; }

        public EntryEditorWithIconViewModel OtherAlphabetViewModel { get; }

        public EntryEditorWithIconViewModel TranslationViewModel { get; }

        public PickerViewViewModel LanguagePickerViewModel { get; }

        public PickerViewViewModel AlphabetPickerViewModel { get; }

        public ICommand AddTapped { get; }

        public ICommand CancelTapped { get; }

        public AddTranslationPopupViewModel()
        {
            CancelTapped = new Command(async () => await Shell.Current.Navigation.PopAllPopupAsync());

            AddTapped = new Command(async () =>
            {
                // Check validation flags
                var isValid =
                    LanguagePickerViewModel.IsValid &&
                    AlphabetPickerViewModel.IsValid &&
                    (!LanguagePickerViewModel.OtherVisible || OtherLanguageViewModel.IsValid) &&
                    (!AlphabetPickerViewModel.OtherVisible || OtherAlphabetViewModel.IsValid);

                // Stop user proceding
                if (!isValid)
                {
                    DependencyService.Get<IAlertDialog>().ShowAlert(
                        (string)Application.Current.Resources["alert_error"],
                        (string)Application.Current.Resources["lang_validation_fail"],
                        new AlertDialogButton
                        {
                            Text = (string)Application.Current.Resources["alert_ok"]
                        }
                    );
                    return;
                }

                // Add model
                var model = new TranslationModel(
                        ConfigurationService.Instance.GetDescriptorFromName(LanguagePickerViewModel.SelectedItem, DescriptorType.Language)?.Code,
                        ConfigurationService.Instance.GetDescriptorFromName(AlphabetPickerViewModel.SelectedItem, DescriptorType.Alphabet)?.Code,
                        TranslationViewModel.EntryText?.Trim(),
                        OtherLanguageViewModel.EntryText?.Trim(),
                        OtherAlphabetViewModel.EntryText?.Trim()
                        );
                SubmissionService.Instance.AddTranslation(model);

                // Send message that a translation has been completed
                MessagingCenter.Send(this, (string)Application.Current.Resources["msg_translation_add"], model);

                // Pop the popup
                await Shell.Current.Navigation.PopAllPopupAsync();
            });

            // Create view models
            LanguagePickerViewModel = new PickerViewViewModel
            {
                Placeholder = (string)Application.Current.Resources["lang_pop_lang"],
                ItemsSource = ConfigurationService.Instance.GetDescriptors(DescriptorType.Language).Select(i => i.Name).ToList(),
                TooltipIconVisible = false,
                CheckHasValue = true
            };
            AlphabetPickerViewModel = new PickerViewViewModel
            {
                Placeholder = (string)Application.Current.Resources["lang_pop_alpha"],
                ItemsSource = ConfigurationService.Instance.GetDescriptors(DescriptorType.Alphabet).Select(i => i.Name).ToList(),
                TooltipIconVisible = false,
                CheckHasValue = true
            };
            OtherLanguageViewModel = new EntryEditorWithIconViewModel
            {
                Placeholder = (string)Application.Current.Resources["lang_pop_other_lang"],
                CheckHasValue = true
            };
            OtherAlphabetViewModel = new EntryEditorWithIconViewModel
            {
                Placeholder = (string)Application.Current.Resources["lang_pop_other_alpha"],
                CheckHasValue = true
            };
            TranslationViewModel = new EntryEditorWithIconViewModel
            {
                Placeholder = (string)Application.Current.Resources["lang_pop_trans"]
            };
        }
    }
}
