using LinguaSnapp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.ViewModels.ContentViews
{
    class DynamicHeaderViewModel : BaseViewModel
    {
        private bool headerLogoVisible = true;
        public bool HeaderLogoVisible { get => headerLogoVisible; set => SetProperty(ref headerLogoVisible, value); }
    }
}
