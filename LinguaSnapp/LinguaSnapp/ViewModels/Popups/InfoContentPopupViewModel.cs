using LinguaSnapp.Models;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.Popups
{
    class InfoContentPopupViewModel : BaseViewModel
    {
        // Here to avoid binding errors -- never actually used as popups don't display the info bar
        public IconLabelButtonViewModel AccessButtonViewModel { get; }

        // Here to avoid binding errors -- never actually used as popups don't display the info bar
        public IconLabelButtonViewModel DataButtonViewModel { get; }

        public ICommand OkTapped { get; }

        public InfoContentPopupViewModel()
        {
            OkTapped = new Command(async () => await Shell.Current.Navigation.PopAllPopupAsync());
        }
    }
}
