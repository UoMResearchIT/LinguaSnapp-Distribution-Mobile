using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LinguaSnapp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(LinguaSnapp.Droid.Impl.PdfWriterImpl))]
namespace LinguaSnapp.Droid.Impl
{
    class PdfWriterImpl : IPdfWriter
    {
        public void Init()
        {
            MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource.ImageSourceImpl = new PdfImageSource();
        }
    }
}