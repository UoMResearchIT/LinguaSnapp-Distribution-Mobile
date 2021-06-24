using LinguaSnapp.ViewModels;
using LinguaSnapp.ViewModels.ContentViews;
using LinguaSnapp.ViewModels.Popups;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LinguaSnapp.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MultiPickPopup : PopupPage
    {
        internal MultiPickPopup(MultiPickerViewViewModel vm)
        {
            InitializeComponent();

            // Set desired BC
            BindingContext = vm;
        }

        // Toggle
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as MultiPickItemViewModel;
            if (item != null) item.ItemChecked = !item.ItemChecked;
        }
    }
}