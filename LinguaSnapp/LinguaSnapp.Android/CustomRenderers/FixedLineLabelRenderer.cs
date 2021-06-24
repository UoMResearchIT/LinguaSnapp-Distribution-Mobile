using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using LinguaSnapp.Controls;
using LinguaSnapp.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FixedLineLabel), typeof(FixedLineLabelRenderer))]
namespace LinguaSnapp.Droid.CustomRenderers
{
    class FixedLineLabelRenderer : LabelRenderer
    {
        public FixedLineLabelRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            // Format the new control
            FormatLabel(e.NewElement as FixedLineLabel);

            // Subscribe to layout changes
            if (Control != null)
            {
                Control.LayoutChange += (s, args) => FormatLabel(e.NewElement as FixedLineLabel);
            }
        }

        private void FormatLabel(FixedLineLabel element)
        {
            if (Control != null && element != null)
            {
                Control.SetSingleLine(element.NumberOfLines == 1);
                Control.SetMaxLines(element.NumberOfLines);
                Control.SetHorizontallyScrolling(false);
                Control.Ellipsize = TextUtils.TruncateAt.End;
            }
        }
    }
}