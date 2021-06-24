using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("uk.ac.manchester")]
[assembly: ExportEffect(typeof(LinguaSnapp.iOS.Effects.AutoSizeLabelEffect), nameof(LinguaSnapp.iOS.Effects.AutoSizeLabelEffect))]
namespace LinguaSnapp.iOS.Effects
{
    class AutoSizeLabelEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Control is UILabel label)
                {
                    label.AdjustsFontSizeToFitWidth = true;
                    label.Lines = 1;
                    label.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
                    label.LineBreakMode = UILineBreakMode.Clip;
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