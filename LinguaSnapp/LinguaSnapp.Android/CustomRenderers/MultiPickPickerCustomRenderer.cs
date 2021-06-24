using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using LinguaSnapp.Controls;
using LinguaSnapp.Droid.CustomRenderers;
using LinguaSnapp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Material.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MultiPickPicker), typeof(MultiPickPickerCustomRenderer))]
namespace LinguaSnapp.Droid.CustomRenderers
{
    class MultiPickPickerCustomRenderer : MaterialEditorRenderer
    {
        public MultiPickPickerCustomRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Control != null)
            {
                var edittext = (EditText)Control.EditText;
                edittext.SetTextIsSelectable(false);
                edittext.SetSelectAllOnFocus(false);
                edittext.SetCursorVisible(false);
                edittext.ShowSoftInputOnFocus = false;
            }
        }
    }
}