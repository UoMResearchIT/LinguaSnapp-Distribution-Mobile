using LinguaSnapp.ViewModels.ContentViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Controls
{
    public class MultiPickPicker : Editor
    {
        public MultiPickPicker()
        {
            this.SetBinding(Editor.PlaceholderProperty, nameof(MultiPickerViewViewModel.Placeholder));
            this.SetBinding(Editor.TextProperty, nameof(MultiPickerViewViewModel.Text));
            AutoSize = EditorAutoSizeOption.TextChanges;
            Focused += async (s, e) =>
            {
                await (BindingContext as MultiPickerViewViewModel)?.ShowMultiPickPopupAsync();
            };

            // Hack as can't get touch event in the renderer on iOS
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        await (BindingContext as MultiPickerViewViewModel)?.ShowMultiPickPopupAsync();
                    }
                })
            });
        }
    }
}
