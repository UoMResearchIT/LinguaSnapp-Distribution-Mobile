using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Text;
using UIKit;
using LinguaSnapp.Interfaces;
using LinguaSnapp.iOS.Impl;

[assembly: Dependency(typeof(LinguaSnapp.iOS.PdfWriterImpl))]
namespace LinguaSnapp.iOS
{
    class PdfWriterImpl : IPdfWriter
    {
        public void Init()
        {
            MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource.ImageSourceImpl = new PdfImageSource();
        }
    }
}