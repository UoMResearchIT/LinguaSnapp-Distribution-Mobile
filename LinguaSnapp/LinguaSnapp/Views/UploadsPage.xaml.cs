using LinguaSnapp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LinguaSnapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UploadsPage : ContentPage
    {
        public UploadsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Do on background thread
            await Task.Run(async () => await (BindingContext as ISubmissionHandler)?.RefreshSubmissionsAsync());
        }
    }
}