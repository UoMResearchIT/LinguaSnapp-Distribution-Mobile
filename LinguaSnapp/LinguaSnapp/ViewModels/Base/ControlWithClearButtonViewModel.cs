using LinguaSnapp.ViewModels.ContentViews;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.Base
{
    abstract class ControlWithClearButtonViewModel : ControlViewModel
    {
        public IconLabelButtonViewModel ClearButtonViewModel { get; }

        internal ControlWithClearButtonViewModel()
        {
            ClearButtonViewModel = new IconLabelButtonViewModel
            {
                LabelText = (string)Application.Current.Resources["clear_button_label"],
                ImageSource = ImageSource.FromFile("ic_cancel_primary"),
                LabelVisible = false,
                IconVisible = false,
                IconSize = 36,
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"],
                TappedCommand = new Command(() => ClearTapped())
            };
        }

        protected abstract void ClearTapped();
    }
}
