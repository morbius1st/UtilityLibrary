using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;


namespace UtilityLibrary
{
	public static class CsUtilitiesMedia
	{
		// load an image from embeded resource
		public static BitmapImage GetBitmapImage(string imageName, string namespacePrefix)
		{
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(namespacePrefix + "." + imageName);

			BitmapImage img = new BitmapImage();

			img.BeginInit();
			img.StreamSource = s;
			img.EndInit();

			return img;

		}

		public static Image GetImage(string imageName, string namespacePrefix)
		{
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(namespacePrefix + "." + imageName);

			return s == null ? null :new Bitmap(s);
		}
	}
}


