using LinguaSnapp.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Controls
{
    class EditorWithValidity : Editor
    {
        public static BindableProperty IsValidProperty = BindableProperty.Create(
            nameof(IsValid),
            typeof(bool),
            typeof(EditorWithValidity),
            defaultBindingMode: BindingMode.TwoWay
        );

        public bool IsValid { get => (bool)GetValue(IsValidProperty); set => SetValue(IsValidProperty, value); }

        public static BindableProperty CheckHasValueProperty = BindableProperty.Create(
            nameof(CheckHasValue),
            typeof(bool),
            typeof(EditorWithValidity),
            propertyChanged: (b, o, n) => ForceValidation(b, o, n)
        );

        public bool CheckHasValue { get => (bool)GetValue(CheckHasValueProperty); set => SetValue(CheckHasValueProperty, value); }

        private static void ForceValidation(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as EditorWithValidity;
            control?.OnTextChanged(control.Text, control.Text);
        }

        public EditorWithValidity()
        {
            // Add the validator
            Behaviors.Add(new EditorWithValidityValidator());
        }
    }
}
