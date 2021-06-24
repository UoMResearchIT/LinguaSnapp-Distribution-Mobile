using LinguaSnapp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.ViewModels.ContentViews
{
    class PickerViewViewModel : ControlWithToolTipViewModel
    {
        public IEnumerable<string> ItemsSource { get; set; }

        private string selectedItem;
        public string SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
                ClearButtonViewModel.IconVisible = !string.IsNullOrWhiteSpace(value) && !IsReadOnly;
                OtherVisible = value?.ToUpper().Trim() == "OTHER";
            }
        }

        private bool otherVisible = false;
        public bool OtherVisible { get => otherVisible; set => SetProperty(ref otherVisible, value); }

        public string Placeholder { get; set; }

        protected override void ClearTapped()
        {
            SelectedItem = null;
        }
    }
}
