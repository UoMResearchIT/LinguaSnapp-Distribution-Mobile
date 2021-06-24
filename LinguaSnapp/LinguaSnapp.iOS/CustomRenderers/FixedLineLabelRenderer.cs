using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using LinguaSnapp.Controls;
using LinguaSnapp.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FixedLineLabel), typeof(FixedLineLabelRenderer))]
namespace LinguaSnapp.iOS.CustomRenderers
{
    class FixedLineLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var fllab = e.NewElement as FixedLineLabel;
                Control.Lines = fllab.NumberOfLines;
                Control.LineBreakMode = UILineBreakMode.TailTruncation;
            }
            
        }
    }
}