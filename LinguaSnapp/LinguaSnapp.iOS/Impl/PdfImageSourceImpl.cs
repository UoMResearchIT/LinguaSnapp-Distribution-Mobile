using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIKit;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace LinguaSnapp.iOS.Impl
{
	internal class PdfImageSourceImpl : IImageSource
	{
		internal UIImage Image;

		public int Width { get; }
		public int Height { get; }
		public string Name { get; }

		public bool Transparent => false;

		public PdfImageSourceImpl(string name, Func<Stream> streamSource, int quality)
		{
			Name = name;
			using (var stream = streamSource.Invoke())
			{
				Image = UIImage.LoadFromData(NSData.FromStream(stream));
				var size = Image?.Size ?? new CoreGraphics.CGSize(0, 0);

				Width = (int)size.Width;
				Height = (int)size.Height;
			}
		}

		public void SaveAsJpeg(MemoryStream ms)
		{
			var jpg = Image.AsJPEG();
			ms.Write(jpg.ToArray(), 0, (int)jpg.Length);
			ms.Seek(0, SeekOrigin.Begin);
		}

		public void SaveAsPdfBitmap(MemoryStream ms)
		{
			throw new NotImplementedException();
		}
	}
}