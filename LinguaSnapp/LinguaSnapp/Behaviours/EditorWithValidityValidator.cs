using LinguaSnapp.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Behaviours
{
    class EditorWithValidityValidator : Behavior<EditorWithValidity>
    {
        protected override void OnAttachedTo(EditorWithValidity inputView)
        {
            inputView.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(inputView);
        }

        protected override void OnDetachingFrom(EditorWithValidity inputView)
        {
            inputView.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(inputView);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            // Get control
            var control = (EditorWithValidity)sender;

            // Perform validation based on relevant properties
            bool isValid = true;
            if (control.CheckHasValue)
            {
                isValid = !string.IsNullOrWhiteSpace(args.NewTextValue);
            }

            // Set appearance
            control.BackgroundColor = isValid ? (Color)Application.Current.Resources["Tertiary"] : (Color)Application.Current.Resources["Error"];
            if (control.Placeholder != null)
            {
                var str = (string)Application.Current.Resources["validation_required"];
                if (isValid)
                {
                    control.Placeholder = control.Placeholder.Replace($" {str}", "");
                }
                else
                {
                    if (!control.Placeholder.Contains(str)) control.Placeholder = $"{control.Placeholder} {str}";
                }
            }

            // Set validation flag property
            control.IsValid = isValid;
        }
    }
}
