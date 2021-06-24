using Foundation;
using LinguaSnapp.iOS.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using System.ComponentModel;
using Xamarin.Forms.Material.iOS;
using CoreGraphics;
using MDS.Essentials.Shared.ViewExtensions;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FrameWithElevation), typeof(FrameWithElevationRenderer))]
namespace LinguaSnapp.iOS.CustomRenderers
{
    class FrameWithElevationRenderer : MaterialFrameRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null && e.NewElement != null)
            {
                UpdateShadow();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Elevation")
            {
                UpdateShadow();
            }
        }

        private void UpdateShadow()
        {

            var materialFrame = (FrameWithElevation)Element;

            // Update shadow to match better material design standards of elevation
            Layer.ShadowRadius = materialFrame?.Elevation ?? 2;
            Layer.ShadowColor = UIColor.Gray.CGColor;
            Layer.ShadowOffset = new CGSize(materialFrame?.Elevation ?? 2, materialFrame?.Elevation ?? 2);
            Layer.ShadowOpacity = 0.60f;
            Layer.MasksToBounds = false;

            // The shadow path should take into account the frame path
            Layer.ShadowPath = UIBezierPath.FromRoundedRect(Layer.Bounds, materialFrame.CornerRadius).CGPath;

            // Make sure the shadow is properly invisible
            if (!materialFrame?.HasShadow ?? false || materialFrame?.Elevation == 0)
            {
                Layer.ShadowOpacity = 0f;
            }
        }
    }
}