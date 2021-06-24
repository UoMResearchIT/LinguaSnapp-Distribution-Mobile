using LinguaSnapp.ViewModels.ContentViews;
using LinguaSnapp.ViewModels.Popups;
using LinguaSnapp.Views.Popups;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.Base
{
    abstract class ControlWithToolTipViewModel : ControlWithClearButtonViewModel
    {
        public IconLabelButtonViewModel ToolTipButtonViewModel { get; }

        public string TooltipPopupTitle { get; set; }

        public FormattedString TooltipPopupBody { get; set; }

        private bool tooltipIconVisible;
        public bool TooltipIconVisible
        {
            get => tooltipIconVisible;
            set
            {
                SetProperty(ref tooltipIconVisible, value);
                if (ToolTipButtonViewModel != null) ToolTipButtonViewModel.IconVisible = value;
            }
        }

        public ControlWithToolTipViewModel()
        {
            ToolTipButtonViewModel = new IconLabelButtonViewModel
            {
                LabelVisible = false,
                ImageSource = ImageSource.FromFile("ic_help_primary"),
                IconSize = 36,
                LabelText = (string)Application.Current.Resources["tooltip_help"],
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"],
                TappedCommand = new Command(async () =>
                    await Shell.Current.Navigation.PushPopupAsync(new ToolTipPopup(new ToolTipPopupViewModel
                    {
                        TooltipTitle = TooltipPopupTitle,
                        TooltipBody = TooltipPopupBody
                    }))
                )
            };
        }
    }
}
