using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.ViewModels.Base
{
    abstract class ControlViewModel : BaseViewModel
    {
        private bool isReadOnly;
        public bool IsReadOnly
        {
            get => isReadOnly;
            set => SetProperty(ref isReadOnly, value);
        }

        private bool isValid;
        public bool IsValid
        {
            get => isValid;
            set => SetProperty(ref isValid, value);
        }

        private bool checkHasValue;
        public bool CheckHasValue
        {
            get => checkHasValue;
            set => SetProperty(ref checkHasValue, value);
        }

        private bool checkPositiveInteger;
        public bool CheckPositiveInteger
        {
            get => checkPositiveInteger;
            set => SetProperty(ref checkPositiveInteger, value);
        }
    }
}
