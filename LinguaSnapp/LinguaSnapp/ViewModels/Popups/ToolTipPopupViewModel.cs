using LinguaSnapp.ViewModels.Base;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.Popups
{
    class ToolTipPopupViewModel : BaseViewModel
    {
        public string TooltipTitle { get; set; }

        public FormattedString TooltipBody { get; set; }

        public ICommand OkTapped { get; }

        public ToolTipPopupViewModel()
        {
            OkTapped = new Command(async () =>
            {
                // Dismiss the popup
                await Shell.Current.Navigation.PopAllPopupAsync();
            });
        }
    }
}
