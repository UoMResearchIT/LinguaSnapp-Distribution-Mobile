using LinguaSnapp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.ContentViews
{
    class IconLabelButtonViewModel : BaseViewModel
    {
        private ICommand tappedCommand;
        public ICommand TappedCommand { get => tappedCommand; set => SetProperty(ref tappedCommand, value); }

        // Image Properties //

        private ImageSource imageSource;
        public ImageSource ImageSource { get => imageSource; set => SetProperty(ref imageSource, value); }

        // Text Properties //

        private double labelSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));
        public double LabelSize { get => labelSize; set => SetProperty(ref labelSize, value); }

        private bool labelVisible = true;
        public bool LabelVisible { get => labelVisible; set => SetProperty(ref labelVisible, value); }

        private string labelText = "Placeholder";
        public string LabelText { get => labelText; set => SetProperty(ref labelText, value); }

        private Color labelColour = (Color)Application.Current.Resources["Primary"];
        public Color LabelColour { get => labelColour; set => SetProperty(ref labelColour, value); }

        // Icon Properties //      

        private bool iconVisible = true;
        public bool IconVisible { get => iconVisible; set => SetProperty(ref iconVisible, value); }

        private double iconCornerRadius = 24;
        public double IconCornerRadius { get => iconCornerRadius; set => SetProperty(ref iconCornerRadius, value); }

        private double iconSize = 48;
        public double IconSize
        {
            get => iconSize;
            set
            {
                SetProperty(ref iconSize, value);
                UpdateLayoutProperties();
            }
        }

        private double iconPadding;
        public double IconPadding { get => iconPadding; set => SetProperty(ref iconPadding, value); }

        private Color iconBackgroundColour = Color.White;
        public Color IconBackgroundColour { get => iconBackgroundColour; set => SetProperty(ref iconBackgroundColour, value); }

        private Color iconBorderColour = Color.White;
        public Color IconBorderColour { get => iconBorderColour; set => SetProperty(ref iconBorderColour, value); }

        private bool hasShadow = true;
        public bool HasShadow { get => hasShadow; set => SetProperty(ref hasShadow, value); }

        private void UpdateLayoutProperties()
        {
            // Corner radius must be half the width and height for it to remain a circle
            IconCornerRadius = IconSize / 2;

            // Padding must allow for content grid to fit into circle
            IconPadding = 0.14645 * IconSize;
        }
    }
}
