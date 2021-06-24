using LinguaSnapp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LinguaSnapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LanguagePage : ContentPage
    {
        public LanguagePage()
        {
            InitializeComponent();

            // Subscribe to messages from VM to scroll translation card view
            MessagingCenter.Subscribe<LanguagePageViewModel, int>(
                this,
                (string)Application.Current.Resources["msg_scroll_trans_cards"],
                (vm, idx) => ScrollCarouselTo(idx)
            );
        }

        private void ScrollCarouselTo(int idx)
        {
            Device.BeginInvokeOnMainThread(() => TranslationCarousel.ScrollTo(idx, position: ScrollToPosition.End));
        }
    }
}