using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Android.Graphics.Bitmap;
using static Android.Graphics.BitmapFactory;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace LinguaSnapp.Droid.Impl
{
	internal class PdfImageSourceImpl : IImageSource
	{
		internal Bitmap Bitmap { get; set; }

		private readonly int _quality;

		public int Width { get; }
		public int Height { get; }
		public string Name { get; }

		public bool Transparent => false;

		public PdfImageSourceImpl(string name, Func<Stream> streamSource, int quality)
		{
			Name = name;
			_quality = quality;
			using (var stream = streamSource.Invoke())
			{
				stream.Seek(0, SeekOrigin.Begin);
				var options = new BitmapFactory.Options();
				Bitmap = DecodeStream(stream, null, options);
				Width =  options.OutWidth;
				Height = options.OutHeight;
			}
		}

		public void SaveAsJpeg(MemoryStream ms)
		{
			Bitmap.Compress(CompressFormat.Jpeg, _quality, ms);
			string encoded = Base64.EncodeToString(ms.ToArray(), Base64Flags.Default);
		}

		public void SaveAsPdfBitmap(MemoryStream ms)
		{
			throw new NotImplementedException();
		}
	}
}