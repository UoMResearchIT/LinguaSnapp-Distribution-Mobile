using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("uk.ac.manchester")]
[assembly: ExportEffect(typeof(LinguaSnapp.Droid.Effects.AutoSizeLabelEffect), nameof(LinguaSnapp.Droid.Effects.AutoSizeLabelEffect))]
namespace LinguaSnapp.Droid.Effects
{
    class AutoSizeLabelEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Control is TextView textView)
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        textView.SetAutoSizeTextTypeUniformWithConfiguration(
                            1,
                            1000,
                            1,
                            (int)ComplexUnitType.Dip);
                    }
                    else
                    {
                        TextViewCompat.SetAutoSizeTextTypeUniformWithConfiguration(
                            textView,
                            1,
                            10,
                            1,
                            (int)ComplexUnitType.Dip);
                    }
                }
            }
            catch
            {
            }
        }

        protected override void OnDetached()
        {
        }
    }
}