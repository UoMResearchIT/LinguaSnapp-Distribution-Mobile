using LinguaSnapp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.ViewModels.Popups
{
    class MultiPickItemViewModel : BaseViewModel
    {
        private bool itemChecked;
        public bool ItemChecked { get => itemChecked; set => SetProperty(ref itemChecked, value); }

        private string itemLabel;
        public string ItemLabel { get => itemLabel; set => SetProperty(ref itemLabel, value); }

        public string Code { get; }

        internal MultiPickItemViewModel(string code, string label, bool initChecked = false)
        {
            Code = code;
            ItemLabel = label;
            ItemChecked = initChecked;
        }
    }
}
