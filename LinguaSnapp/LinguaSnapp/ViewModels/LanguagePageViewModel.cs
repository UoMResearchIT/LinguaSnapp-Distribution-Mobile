using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using LinguaSnapp.ViewModels.Popups;
using LinguaSnapp.Views.Popups;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels
{
    class LanguagePageViewModel : EditorViewModel
    {
        public EntryEditorWithIconViewModel PhotoTitleViewModel { get; }

        public EntryEditorWithIconViewModel NumLangViewModel { get; }

        public EntryEditorWithIconViewModel NumAlphaViewModel { get; }

        public IconLabelButtonViewModel AddTranslationButton { get; }

        public ObservableCollection<TranslationCardViewModel> TranslationCardViewModels { get; } = new ObservableCollection<TranslationCardViewModel>();

        private bool isCarouselEmpty = true;
        public bool IsCarouselEmpty
        {
            get => isCarouselEmpty;
            private set
            {
                SetProperty(ref isCarouselEmpty, value);
                UpdateSummaryText();
            }
        }

        private FormattedString detailSummaryText;
        public FormattedString DetailSummaryText { get => detailSummaryText; private set => SetProperty(ref detailSummaryText, value); }

        public LanguagePageViewModel()
        {
            AddTranslationButton = new IconLabelButtonViewModel
            {
                ImageSource = ImageSource.FromFile("ic_add_primary"),
                LabelSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                LabelText = (string)Application.Current.Resources["lang_add"],
                TappedCommand = new Command(async () => await Shell.Current.Navigation.PushPopupAsync(new AddTranslationPopup())),
                IconVisible = !IsReadOnly,
                IconSize = 128,
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"],
            };

            // Subscribe to requests to add/delete a translation from the active submission
            MessagingCenter.Subscribe<TranslationCardViewModel, string>(this,
                (string)Application.Current.Resources["msg_translation_delete"],
                (vm, id) =>
                {
                    // Remove the card
                    TranslationCardViewModels.Remove(vm);

                    // Set flag
                    IsCarouselEmpty = TranslationCardViewModels.Count == 0;
                });
            MessagingCenter.Subscribe<AddTranslationPopupViewModel, TranslationModel>(this,
                (string)Application.Current.Resources["msg_translation_add"],
                (vm, model) =>
                {
                    // Add the card
                    TranslationCardViewModels.Add(new TranslationCardViewModel(model, IsReadOnly));

                    // Set flag
                    IsCarouselEmpty = TranslationCardViewModels.Count == 0;

                    // Fire message off to scroll carousel
                    MessagingCenter.Send(this,
                        (string)Application.Current.Resources["msg_scroll_trans_cards"],
                        TranslationCardViewModels.Count - 1);
                });

            // Create view models
            PhotoTitleViewModel = new EntryEditorWithIconViewModel()
            {
                Placeholder = (string)Application.Current.Resources["lang_title"],
                IsReadOnly = IsReadOnly,
                CheckHasValue = true
            };
            NumLangViewModel = new EntryEditorWithIconViewModel()
            {
                Placeholder = (string)Application.Current.Resources["lang_num_lang"],
                IsReadOnly = IsReadOnly,
                Keyboard = Keyboard.Numeric,
                CheckPositiveInteger = true
            };
            NumAlphaViewModel = new EntryEditorWithIconViewModel()
            {
                Placeholder = (string)Application.Current.Resources["lang_num_alpha"],
                IsReadOnly = IsReadOnly,
                Keyboard = Keyboard.Numeric,
                CheckPositiveInteger = true
            };

            // Load
            PopulateViewModelFromSubmission();
        }

        // Override load for this VM
        protected override void PopulateViewModelFromSubmission()
        {
            base.PopulateViewModelFromSubmission();

            // Set touched flag
            SubmissionService.Instance.SetTouchedFlag(EditorTabType.Language);

            // Load in the language components from the active submission
            var model = SubmissionService.Instance.GetSubmissionMeta();
            PhotoTitleViewModel.EntryText = model.Title;
            NumLangViewModel.EntryText = model.NumLang;
            NumAlphaViewModel.EntryText = model.NumAlpha;

            // Refresh translation cards
            LoadTranslationCards();
        }

        // Override update for this VM
        protected override void UpdateSubmissionFromViewModel()
        {
            base.UpdateSubmissionFromViewModel();

            // Check validation flags
            var valid = 
                PhotoTitleViewModel.IsValid &&
                NumLangViewModel.IsValid &&
                NumAlphaViewModel.IsValid;

            // Set flag
            SubmissionService.Instance.SetLanguageValidityFlag(valid);

            // Write back changes in fields
            SubmissionService.Instance.SetLanguageMetaData(
                PhotoTitleViewModel.EntryText?.Trim(),
                NumLangViewModel.EntryText?.Trim(),
                NumAlphaViewModel.EntryText?.Trim()
                );
        }

        private void LoadTranslationCards()
        {
            // Clear existing list
            TranslationCardViewModels.Clear();

            // Convert models to ViewModels
            var translations = SubmissionService.Instance.GetTranslations();
            foreach (var t in translations)
            {
                TranslationCardViewModels.Add(new TranslationCardViewModel(t, IsReadOnly));
            }

            // Set flag
            IsCarouselEmpty = TranslationCardViewModels.Count == 0;
        }

        private void UpdateSummaryText()
        {
            DetailSummaryText = new FormattedString
            {
                Spans =
                {
                    new Span
                    {
                        Text = $"{TranslationCardViewModels?.Count}",
                        FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)),
                        TextColor = (Color)Application.Current.Resources["PrimaryLightBackground"]

                    },
                    new Span
                    {
                        Text = $" {(string)Application.Current.Resources["lang_detail"]}"
                    }
                }
            };
        }
    }
}
