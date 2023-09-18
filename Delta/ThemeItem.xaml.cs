using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Delta
{
	public partial class ThemeItem : UserControl
	{
		public ThemeItem(Theme this_theme)
		{
			this.InitializeComponent();
			this.this__theme = this_theme;
			this.name_text.Text = this_theme.title;
			this.status_text.Text = this_theme.status;
			this.imgbrush.ImageSource = LinkOpener.urltoimage(this.this__theme.imageURL);
		}

		void cyan_Checked(object sender, RoutedEventArgs e)
		{
			try
			{
				foreach (object obj in ((XWindow)Application.Current.MainWindow).theme_wrap_panel.Children)
				{
					ThemeItem themeItem = (ThemeItem)obj;
					if (themeItem != this)
					{
						themeItem.cyan.IsChecked = new bool?(false);
					}
				}
				((XWindow)Application.Current.MainWindow).theme_title.Text = this.this__theme.title;
				((XWindow)Application.Current.MainWindow).theme_desc.Text = this.this__theme.description;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Delta Error");
				Directory.CreateDirectory(Path.Combine(this.deltacore, "error_logs"));
				string text = Path.Combine(Path.Combine(new string[]
				{
					Path.Combine(this.deltacore, "error_logs")
				}), "ERROR_LOG_" + DateTime.UtcNow.TimeOfDay.TotalMilliseconds.ToString() + ".txt");
				string contents = string.Format("GO TO OUR DISCORD MAKE A TICKET AND SEND THIS\r\n\r\nDelta Error Log {0}\r\n\r\nFile Name: {1}\r\nHResult: {2}\r\nException Data: {3}\r\nCausing Func: {4}\r\nTarget Site: {5}\r\nOuter Exception Message: {6}\r\nOuter Exception Source: {7}\r\n\r\n\r\nException as String: {8}\r\n", new object[]
				{
					DateTime.Now,
					text,
					ex.HResult,
					ex.Data,
					new StackTrace(ex, true).GetFrame(0).GetMethod().Name,
					ex.TargetSite,
					ex.Message,
					ex.Source,
					ex
				});
				File.WriteAllText(text, contents);
				Process.Start(text);
			}
		}

		string deltacore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "delta_core");

		public Theme this__theme;
	}
}
