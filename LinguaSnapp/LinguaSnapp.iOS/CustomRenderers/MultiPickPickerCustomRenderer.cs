using Foundation;
using LinguaSnapp.Controls;
using LinguaSnapp.iOS.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Material.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MultiPickPicker), typeof(MultiPickPickerCustomRenderer))]
namespace LinguaSnapp.iOS.CustomRenderers
{
    class MultiPickPickerCustomRenderer : MaterialEditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Control != null)
            {
                // Need to stop keyboard and editing of the editor control
                Control.TextView.Editable = false;
                Control.TextView.Selectable = false;
            }
        }
    }
}