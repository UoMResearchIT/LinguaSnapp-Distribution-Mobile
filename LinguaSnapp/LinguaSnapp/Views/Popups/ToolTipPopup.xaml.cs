using LinguaSnapp.ViewModels.Popups;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LinguaSnapp.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ToolTipPopup : PopupPage
    {
        internal ToolTipPopup(ToolTipPopupViewModel vm)
        {
            InitializeComponent();

            // Set desired BC
            BindingContext = vm;
        }
    }
}