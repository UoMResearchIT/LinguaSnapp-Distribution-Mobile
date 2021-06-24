using LinguaSnapp.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Controls
{
    class EntryWithValidity : Entry
    {
        public static BindableProperty IsValidProperty = BindableProperty.Create(
            nameof(IsValid),
            typeof(bool),
            typeof(EntryWithValidity),
            defaultBindingMode: BindingMode.TwoWay
        );

        public bool IsValid { get => (bool)GetValue(IsValidProperty); set => SetValue(IsValidProperty, value); }

        public static BindableProperty CheckHasValueProperty = BindableProperty.Create(
            nameof(CheckHasValue),
            typeof(bool),
            typeof(EntryWithValidity),
            propertyChanged: (b, o, n) => ForceValidation(b, o, n)
        );

        public bool CheckHasValue { get => (bool)GetValue(CheckHasValueProperty); set => SetValue(CheckHasValueProperty, value); }

        public static BindableProperty CheckPositiveIntegerProperty = BindableProperty.Create(
            nameof(CheckPositiveInteger),
            typeof(bool),
            typeof(EntryWithValidity),
            propertyChanged: (b, o, n) => ForceValidation(b, o, n)
        );

        public bool CheckPositiveInteger { get => (bool)GetValue(CheckPositiveIntegerProperty); set => SetValue(CheckPositiveIntegerProperty, value); }

        private static void ForceValidation(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as EntryWithValidity;
            control?.OnTextChanged(control.Text, control.Text);
        }

        public EntryWithValidity()
        {
            // Add the validator
            Behaviors.Add(new EntryWithValidityValidator());
        }
    }
}
