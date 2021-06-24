using LinguaSnapp.Interfaces;
using LinguaSnapp.ViewModels.ContentViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LinguaSnapp.Views.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IconLabelButton : ContentView
    {
        public IconLabelButton()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            // Grow
            await this.ScaleTo(1.1, 100);

            // Shrink after command
            await this.ScaleTo(1.0, 100);

            // Fire command
            var vm = BindingContext as IconLabelButtonViewModel;
            if (vm != null && (vm.TappedCommand?.CanExecute(null) ?? false)) vm.TappedCommand.Execute(null);
        }
    }
}