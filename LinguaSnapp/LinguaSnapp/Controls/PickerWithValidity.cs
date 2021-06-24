using LinguaSnapp.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Controls
{
    class PickerWithValidity : Picker
    {
        public static BindableProperty IsValidProperty = BindableProperty.Create(
            nameof(IsValid),
            typeof(bool),
            typeof(PickerWithValidity),
            defaultBindingMode: BindingMode.TwoWay
        );

        public bool IsValid { get => (bool)GetValue(IsValidProperty); set => SetValue(IsValidProperty, value); }

        public static BindableProperty CheckHasValueProperty = BindableProperty.Create(
            nameof(CheckHasValue),
            typeof(bool),
            typeof(PickerWithValidity),
            propertyChanged: (b, o, n) => ForceValidation(b, o, n)
        );

        public bool CheckHasValue { get => (bool)GetValue(CheckHasValueProperty); set => SetValue(CheckHasValueProperty, value); }

        private static void ForceValidation(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as PickerWithValidity;
            var validator = control.Behaviors.FirstOrDefault(b => b is PickerWithValidityValidator);
            (validator as PickerWithValidityValidator)?.OnValidationRequired(control, new EventArgs());
        }

        public PickerWithValidity()
        {
            // Add the validator
            Behaviors.Add(new PickerWithValidityValidator());
        }
    }
}
