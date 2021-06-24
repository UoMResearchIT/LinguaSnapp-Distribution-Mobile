﻿using LinguaSnapp.Views.ContentViews;
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
    public partial class InfoContentPopup : PopupPage
    {
        public InfoContentPopup(string resource)
        {
            InitializeComponent();

            // Insert the correct view
            PopGrid.Children.Insert(0, new InfoContent(resource));
        }
    }
}