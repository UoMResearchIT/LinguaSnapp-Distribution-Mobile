using LinguaSnapp.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Behaviours
{
    class PickerWithValidityValidator : Behavior<PickerWithValidity>
    {
        protected override void OnAttachedTo(PickerWithValidity picker)
        {
            picker.SelectedIndexChanged += OnValidationRequired;
            base.OnAttachedTo(picker);
        }

        protected override void OnDetachingFrom(PickerWithValidity picker)
        {
            picker.SelectedIndexChanged -= OnValidationRequired;
            base.OnDetachingFrom(picker);
        }

        internal void OnValidationRequired(object sender, EventArgs args)
        {
            // Get control
            var control = (PickerWithValidity)sender;

            // Perform validation based on relevant properties
            bool isValid = true;
            if (control.CheckHasValue)
            {
                isValid = !string.IsNullOrWhiteSpace((string)control.SelectedItem);
            }

            // Set appearance
            control.BackgroundColor = isValid ? (Color)Application.Current.Resources["Tertiary"] : (Color)Application.Current.Resources["Error"];
            if (control.Title != null)
            {
                var str = (string)Application.Current.Resources["validation_required"];
                if (isValid)
                {
                    control.Title = control.Title.Replace($" {str}", "");
                }
                else
                {
                    if (!control.Title.Contains(str)) control.Title = $"{control.Title} {str}";
                }                    
            }

            // Set validation flag property
            control.IsValid = isValid;
        }
    }
}
