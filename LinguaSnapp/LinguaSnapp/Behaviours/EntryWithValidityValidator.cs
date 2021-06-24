using LinguaSnapp.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Behaviours
{
    class EntryWithValidityValidator : Behavior<EntryWithValidity>
    {
        private string originalPlaceholder;

        protected override void OnAttachedTo(EntryWithValidity inputView)
        {
            inputView.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(inputView);
        }

        protected override void OnDetachingFrom(EntryWithValidity inputView)
        {
            inputView.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(inputView);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            // Set original placeholder
            if (string.IsNullOrEmpty(originalPlaceholder))
                originalPlaceholder = (sender as InputView)?.Placeholder;
            
            // Get control
            var control = (EntryWithValidity)sender;

            // Perform validation based on relevant properties
            bool isValid = true;
            if (control.CheckHasValue)
            {
                isValid = !string.IsNullOrWhiteSpace(args.NewTextValue);
            }
            if (isValid && control.CheckPositiveInteger)
            {
                var res = int.TryParse(args.NewTextValue?.Trim(), out var val);
                isValid = res && val > -1;
            }

            // Concat placeholder message
            string str = control.CheckHasValue ?
                (string)Application.Current.Resources["validation_required"] :
                string.Empty;
            if (control.CheckPositiveInteger)
            {
                str = string.IsNullOrEmpty(str) ?
                    (string)Application.Current.Resources["validation_pos_int"] :
                    string.Join(", ", str, (string)Application.Current.Resources["validation_pos_int"]);
            }
            str = $"({str})";

            // Set background colour
            control.BackgroundColor = isValid ?
                (Color)Application.Current.Resources["Tertiary"] :
                (Color)Application.Current.Resources["Error"];

            // Set placholder message
            if (control.Placeholder != null)
            {
                if (isValid)
                {
                    control.Placeholder = originalPlaceholder;
                }
                else
                {
                    control.Placeholder = $"{originalPlaceholder} {str}";
                }
            }

            // Set validation flag property
            control.IsValid = isValid;
        }
    }
}
