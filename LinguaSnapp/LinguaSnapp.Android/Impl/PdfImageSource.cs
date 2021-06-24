using System;
using System.IO;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;


namespace LinguaSnapp.Droid.Impl
{
	public class PdfImageSource : ImageSource
	{
		protected override IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75)
		{
			throw new NotImplementedException();
		}

		protected override IImageSource FromFileImpl(string path, int? quality = 75)
		{
			throw new NotImplementedException();
		}

		protected override IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75)
		{
			return new PdfImageSourceImpl(name, imageStream, (int)quality);
		}
	}
}