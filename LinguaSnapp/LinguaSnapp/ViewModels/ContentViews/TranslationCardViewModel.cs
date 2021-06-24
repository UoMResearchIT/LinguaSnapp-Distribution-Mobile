using System;
using System.Collections.Generic;
using System.Text;
using LinguaSnapp.Models;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.ContentViews
{
    class TranslationCardViewModel : BaseViewModel
    {
        public IconLabelButtonViewModel DeleteIconViewModel { get; }

        public string TranslationId { get; }

        public string Language { get; }

        public string Alphabet { get; }

        public string Translation { get; set; }

        public TranslationCardViewModel(TranslationModel model, bool isReadOnly)
        {
            // Get name from code using the config service
            Language = model.LanguageOtherValue != null ? model.LanguageOtherValue : ConfigurationService.Instance.GetDescriptorFromCode(model.LanguageCode)?.Name;
            Alphabet = model.AlphabetOtherValue != null ? model.AlphabetOtherValue : ConfigurationService.Instance.GetDescriptorFromCode(model.AlphabetCode)?.Name;
            Translation = model.Translation;
            TranslationId = model.TranslationId;
            DeleteIconViewModel = new IconLabelButtonViewModel
            {
                LabelText = (string)Application.Current.Resources["lang_card_delete"],
                ImageSource = "ic_delete_primary",
                TappedCommand = new Command(() =>
                {
                    // Remove model
                    SubmissionService.Instance.RemoveTranslation(TranslationId);

                    // Send message to remove card
                    MessagingCenter.Send(this, (string)Application.Current.Resources["msg_translation_delete"], TranslationId);
                }),
                IconVisible = !isReadOnly,
                IconSize = 56,
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"],
                LabelSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label))
            };
        }
    }
}
