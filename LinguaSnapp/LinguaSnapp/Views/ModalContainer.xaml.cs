using LinguaSnapp.Interfaces;
using LinguaSnapp.ViewModels;
using LinguaSnapp.Views.ContentViews;
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
    public partial class ModalContainer : ContentPage
    {
        public enum PageLayoutType
        {
            Register,
            Login
        }

        public static readonly BindableProperty PageLayoutProperty = BindableProperty.Create(
            nameof(PageLayout),
            typeof(PageLayoutType),
            typeof(ModalContainer),
            defaultValue: PageLayoutType.Register,
            propertyChanged: (b, o, n) => (b as ModalContainer).LayoutPage());

        public PageLayoutType PageLayout
        {
            get { return (PageLayoutType)GetValue(PageLayoutProperty); }
            set { SetValue(PageLayoutProperty, value); }
        }

        public ModalContainer()
        {
            InitializeComponent();
            BindingContext = new ModalContainerViewModel();
            LayoutPage();

            // Subscribe to messages to change view contents
            MessagingCenter.Subscribe<IModalContainerViewModel>(this,
                (string)Application.Current.Resources["msg_register_tapped"],
                vm => PageLayout = PageLayoutType.Register);
            MessagingCenter.Subscribe<IModalContainerViewModel>(this,
                (string)Application.Current.Resources["msg_login_tapped"],
                vm => PageLayout = PageLayoutType.Login);
        }

        private async void LayoutPage()
        {
            // Clear existing content view
            if (ContainerGrid.Children.Count > 1)
            {
                await ContainerGrid.Children[1].FadeTo(0);
                ContainerGrid.Children.RemoveAt(1);
            }

            // Pick appropriate content view
            ContentView content = null;
            switch (PageLayout)
            {
                case PageLayoutType.Login:
                    content = new LoginContent();
                    break;

                case PageLayoutType.Register:
                    content = new RegisterContent();
                    break;
            }

            // Add content, update VM and bring in
            content.Opacity = 0;
            ContainerGrid.Children.Add(content, 0, 1);
            (BindingContext as IModalContainerViewModel)?.Update(PageLayout);
            await ContainerGrid.Children[1].FadeTo(1.0);
        }
    }
}