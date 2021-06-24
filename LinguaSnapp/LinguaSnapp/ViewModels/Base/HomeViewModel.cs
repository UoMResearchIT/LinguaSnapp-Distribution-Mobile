using LinguaSnapp.ViewModels.ContentViews;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.ViewModels.Base
{
    abstract class HomeViewModel : BaseViewModel
    {
        public DynamicHeaderViewModel HeaderViewModel { get; }

        public HomeViewModel()
        {
            HeaderViewModel = new DynamicHeaderViewModel
            {
                HeaderLogoVisible = true
            };
        }
    }
}
