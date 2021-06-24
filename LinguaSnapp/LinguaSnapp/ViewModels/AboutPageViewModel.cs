using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using LinguaSnapp.Views.Popups;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels
{
    class AboutPageViewModel : HomeViewModel
    {
        public IconLabelButtonViewModel AccessButtonViewModel { get; }

        public IconLabelButtonViewModel DataButtonViewModel { get; }

        public AboutPageViewModel()
        {
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
        }
    }
}
