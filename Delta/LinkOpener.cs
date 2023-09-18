using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace Delta
{
	public static class LinkOpener
	{
		public static void openlink(string link)
		{
			try
			{
				Process.Start(new ProcessStartInfo(link)
				{
					UseShellExecute = true,
					Verb = "open"
				});
			}
			catch (Win32Exception)
			{
				Process.Start("IExplore.exe", link);
			}
		}

		public static BitmapImage urltoimage(string url)
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(url, UriKind.Absolute);
			bitmapImage.DecodePixelWidth = 150;
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.EndInit();
			return bitmapImage;
		}
	}
}
