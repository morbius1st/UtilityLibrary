using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;


namespace UtilityLibrary
{
	public static class CsUtilitiesMedia
	{
		// load an image from embeded resource
		/// <summary>
		/// Load an image from embeded resource  [use GetBitmapImageResource instead]
		/// </summary>
		/// <param name="imageName">String name of the image</param>
		/// <param name="namespacePrefix">String Image Namespace</param>
		/// <returns></returns>
		public static BitmapImage GetBitmapImage(string imageName, string namespacePrefix)
		{
			BitmapImage img = new BitmapImage();
			try
			{
				Assembly a = Assembly.GetExecutingAssembly();
				Stream m = a.GetManifestResourceStream(namespacePrefix + "." + imageName);



				Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(namespacePrefix + "." + imageName);

				img.BeginInit();
				img.StreamSource = s;
				img.EndInit();

			}
			catch { }

			return img;

		}


		// BitmapImage b = new BitmapImage(new Uri("pack://application:,,,/CsDeluxMeasure;component/Resources/CyberStudio Icon.png"));

		/// <summary>
		/// get a BitmapImage from an assembly's resource cache (not from the embeded resource cache)
		/// </summary>
		/// <param name="imagePath">The path to the image including a preceding slash<br/>e.g. /folder/imagename.png</param>
		/// <returns></returns>
		public static BitmapImage GetBitmapImageResource(string imagePath)
		{
			string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

			if (assemblyName.IsVoid() || imagePath.IsVoid()) return null;

			string uri = $"pack://application:,,,/{assemblyName};component{imagePath}";

			BitmapImage b = new BitmapImage(new Uri(uri));

			return b;
		}



//		public static MediaTypeNames.Image GetImage(string imageName, string namespacePrefix)
//		{
//			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(namespacePrefix + "." + imageName);
//
//			return s == null ? null :new Bitmap(s);
//		}
	}
}


