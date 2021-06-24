using LinguaSnapp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.ContentViews
{
    class EntryEditorWithIconViewModel : ControlWithClearButtonViewModel
    {
        private ImageSource iconSource;
        public ImageSource IconSource { get => iconSource; set => SetProperty(ref iconSource, value); }

        private string entryText;
        public string EntryText
        {
            get => entryText;
            set
            {
                SetProperty(ref entryText, value);
                ClearButtonViewModel.IconVisible = !string.IsNullOrWhiteSpace(value) && !IsReadOnly;
            }
        }

        private ImageSource statusIconSource;
        public ImageSource StatusIconSource { get => statusIconSource; set => SetProperty(ref statusIconSource, value); }

        private string placeholder;
        public string Placeholder { get => placeholder; set => SetProperty(ref placeholder, value); }

        private bool isPassword;
        public bool IsPassword { get => isPassword; set => SetProperty(ref isPassword, value); }

        private Keyboard keyboard = Keyboard.Default;
        public Keyboard Keyboard { get => keyboard; set => SetProperty(ref keyboard, value); }

        protected override void ClearTapped()
        {
            EntryText = null;
        }
    }
}
