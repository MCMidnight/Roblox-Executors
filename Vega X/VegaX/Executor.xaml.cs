using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json.Linq;
using VegaX.Classes.DLL;
using VegaX.SpawnableSettings;
using VegaX.SpawnableWindows;
using Vega_X.Classes;
using WpfAnimatedGif;

namespace VegaX
{
	public partial class Executor : Window
	{
		IEasingFunction Smooth { get; set; } = new QuarticEase
		{
			EasingMode = EasingMode.EaseInOut
		};

		public void Fade(DependencyObject Object)
		{
			DoubleAnimation doubleAnimation = new DoubleAnimation
			{
				From = new double?(0.0),
				To = new double?(1.0),
				Duration = new Duration(this.duration)
			};
			Storyboard.SetTarget(doubleAnimation, Object);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity", new object[]
			{
				1
			}));
			this.StoryBoard.Children.Add(doubleAnimation);
			this.StoryBoard.Begin();
			this.StoryBoard.Children.Remove(doubleAnimation);
		}

		public void FadeOut(DependencyObject Object)
		{
			DoubleAnimation doubleAnimation = new DoubleAnimation
			{
				From = new double?(1.0),
				To = new double?(0.0),
				Duration = new Duration(this.duration)
			};
			Storyboard.SetTarget(doubleAnimation, Object);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity", new object[]
			{
				1
			}));
			this.StoryBoard.Children.Add(doubleAnimation);
			this.StoryBoard.Begin();
			this.StoryBoard.Children.Remove(doubleAnimation);
		}

		public void ObjectShift(Duration speed, DependencyObject Object, Thickness Get, Thickness Set)
		{
			ThicknessAnimation thicknessAnimation = new ThicknessAnimation
			{
				From = new Thickness?(Get),
				To = new Thickness?(Set),
				Duration = speed,
				EasingFunction = this.Smooth
			};
			Storyboard.SetTarget(thicknessAnimation, Object);
			Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath(FrameworkElement.MarginProperty));
			this.StoryBoard.Children.Add(thicknessAnimation);
			this.StoryBoard.Begin();
			this.StoryBoard.Children.Remove(thicknessAnimation);
		}

		public void BorderShifting(Duration time, Border Object, DependencyProperty Property, double startingpos, double endingpos)
		{
			Object.BeginAnimation(Property, new DoubleAnimation
			{
				From = new double?(startingpos),
				To = new double?(endingpos),
				Duration = time,
				EasingFunction = new QuarticEase()
			});
		}

		public Executor()
		{
			this.InitializeComponent();
			try
			{
				using (WebClient webClient = new WebClient())
				{
					if (webClient.DownloadString(new Uri("https://vegax.gg/windows/ui_ver.php")) != Executor.current_ui_ver)
					{
						if (System.Windows.MessageBox.Show("You are running an old version of Vega X!\nWould you like to re-download the new Vega X?", "Vega X", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
						{
							this.OpenProcess("https://vegax.gg/");
						}
						Process.GetCurrentProcess().Kill();
					}
				}
			}
			catch
			{
				if (System.Windows.MessageBox.Show("Your Internet Service Provider (ISP) appears to be blocking the connection to Vega X Servers.\n\nUnfortunately, Vega X cannot run without a proper connection to https://vegax.gg/\n\n---\n\n[TO FIX THIS ISSUE]\n\nYou will need to use a VPN Service like ProtonVPN and run the VPN while using Vega X.\n\nWe provide a tutorial video on how to do this, would you like to view it?", "Vega X", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
				{
					this.OpenProcess("https://vegax.gg/");
				}
			}
			this.VegaXTitle.Content = this.VegaXTitleContent;
			this.AutoInject = new DispatcherTimer(TimeSpan.FromSeconds(5.0), DispatcherPriority.Normal, delegate(object <p0>, EventArgs <p1>)
			{
				Executor.<>c.<<-ctor>b__17_0>d <<-ctor>b__17_0>d;
				<<-ctor>b__17_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<-ctor>b__17_0>d.<>1__state = -1;
				<<-ctor>b__17_0>d.<>t__builder.Start<Executor.<>c.<<-ctor>b__17_0>d>(ref <<-ctor>b__17_0>d);
			}, System.Windows.Application.Current.Dispatcher);
			this.InjectCheck = new DispatcherTimer(TimeSpan.FromSeconds(2.0), DispatcherPriority.Normal, delegate(object <p0>, EventArgs <p1>)
			{
				if (FluxInteractionsAPI.is_injected())
				{
					this.VegaXTitle.Content = this.VegaXTitleContent + " - Injected";
					return;
				}
				this.VegaXTitle.Content = this.VegaXTitleContent + " - Not Injected";
			}, System.Windows.Application.Current.Dispatcher);
			if (!Directory.Exists("bin\\favoritedscripts"))
			{
				Directory.CreateDirectory("bin\\favoritedscripts");
			}
			if (!Directory.Exists("add_scripts_here"))
			{
				Directory.CreateDirectory("add_scripts_here");
			}
			if (!File.Exists("bin\\highlighter.xshd") || !File.Exists("bin\\FluxInteractions.dll"))
			{
				if (System.Windows.MessageBox.Show("Failed to locate required files!\nThis could be due to an anti virus, would you like to re-download Vega X?", "Vega X", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
				{
					this.OpenProcess("https://vegax.gg/");
				}
				Process.GetCurrentProcess().Kill();
			}
			if (!Directory.Exists(DLLFileSystem.RBPath + "\\ipc"))
			{
				Directory.CreateDirectory(DLLFileSystem.RBPath + "\\ipc");
			}
			if (File.Exists(DLLFileSystem.AutoexecPath + "setclipboard_system.txt"))
			{
				File.Delete(DLLFileSystem.AutoexecPath + "setclipboard_system.txt");
			}
			if (!File.Exists(DLLFileSystem.RBPath + "\\ipc\\clip.txt"))
			{
				File.WriteAllText(DLLFileSystem.RBPath + "\\ipc\\clip.txt", "");
			}
			this.RefreshScriptList();
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
			fileSystemWatcher.IncludeSubdirectories = false;
			fileSystemWatcher.Path = ".\\add_scripts_here\\";
			fileSystemWatcher.Created += this.FileSystemWatcher_Created;
			fileSystemWatcher.Renamed += new RenamedEventHandler(this.FileSystemWatcher_Renamed);
			fileSystemWatcher.Deleted += this.FileSystemWatcher_Deleted;
			fileSystemWatcher.EnableRaisingEvents = true;
			this.tabEdit = new TabEdit(this.TabCtrl);
			this.TabCtrl.Loaded += delegate(object sender, RoutedEventArgs e)
			{
				this.TabCtrl.GetTemplateChild("AddTabButton").Click += delegate(object sender1, RoutedEventArgs e1)
				{
					this.TabCounter++;
					this.MakeTab("", "Script " + this.TabCounter.ToString());
				};
			};
			base.Closing += delegate(object sender, CancelEventArgs e)
			{
				this.CloseWindows();
			};
			this.TabCounter++;
			this.MakeTab("", "Script " + this.TabCounter.ToString());
			this.ExecutorMainWindow.Drop += delegate(object s, System.Windows.DragEventArgs e)
			{
				try
				{
					string[] array = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
					string extension = System.IO.Path.GetExtension(array[0]);
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.UriSource = new Uri(array[0]);
					bitmapImage.EndInit();
					this.ExitBackground.CornerRadius = new CornerRadius(0.0, 0.0, 0.0, 0.0);
					this.ImageBg.Visibility = Visibility.Visible;
					if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
					{
						HandleSettings.SaveString("Image_Background", array[0]);
						ImageBehavior.SetAnimatedSource(this.ImageBg, bitmapImage);
					}
					else if (extension == ".gif")
					{
						HandleSettings.SaveString("Image_Background", array[0]);
						ImageBehavior.SetAnimatedSource(this.ImageBg, bitmapImage);
						ImageBehavior.SetRepeatBehavior(this.ImageBg, RepeatBehavior.Forever);
					}
					else
					{
						HandleSettings.SaveString("Image_Background", "");
						System.Windows.MessageBox.Show("Invalid Image Extension, please convert the image to png, jpg, jpeg, or gif.", "Vega X");
					}
				}
				catch (Exception ex)
				{
					HandleSettings.SaveString("Image_Background", "");
					System.Windows.MessageBox.Show("Vega X cannot read this image.\n\n" + ex.ToString(), "Vega X");
				}
			};
		}

		void Window_Loaded(object sender, RoutedEventArgs e)
		{
			FluxInteractionsAPI.create_files(DLLFileSystem.DLLPath);
			HandleSettings.SetDefaultSettings();
			this.LoadSettings();
			this.LoadAccentColors();
			this.LoadImageBackground();
			try
			{
				FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
				fileSystemWatcher.IncludeSubdirectories = false;
				fileSystemWatcher.Path = DLLFileSystem.RBPath + "\\ipc";
				fileSystemWatcher.Filter = "clip.txt";
				fileSystemWatcher.Changed += delegate(object s, FileSystemEventArgs k)
				{
					base.Dispatcher.Invoke(delegate()
					{
						System.Windows.Clipboard.SetText(File.ReadAllText(DLLFileSystem.RBPath + "\\ipc\\clip.txt"));
					});
				};
				fileSystemWatcher.EnableRaisingEvents = true;
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("Vega X just prevented a crash.\n\n" + ex.ToString(), "Vega X");
			}
			if (HandleSettings.ReadSBool("ScriptBlox_ScriptHub_DefaultPosture"))
			{
				this.GameWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString(HandleSettings.ReadSString("Accent_Color"));
				this.FavoriteWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");
				this.ScriptScroller.Visibility = Visibility.Visible;
				this.FavoriteScriptScroller.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.GameWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");
				this.FavoriteWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#FFFFAC37");
				this.ScriptScroller.Visibility = Visibility.Collapsed;
				this.FavoriteScriptScroller.Visibility = Visibility.Visible;
			}
			this.ScriptHubLoaded = true;
		}

		void Intro_Completed(object sender, EventArgs e)
		{
			base.ResizeMode = ResizeMode.CanResize;
		}

		void Exit_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		void Maximize_Click(object sender, RoutedEventArgs e)
		{
			if (base.WindowState != WindowState.Maximized)
			{
				base.WindowState = WindowState.Maximized;
				return;
			}
			base.WindowState = WindowState.Normal;
		}

		void Minimize_Click(object sender, RoutedEventArgs e)
		{
			base.WindowState = WindowState.Minimized;
		}

		void MainBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			base.DragMove();
		}

		void LoadSettings()
		{
			SSwitch Always_on_Top = new SSwitch("", "Always on Top", "Put Vega X on top of other Windows at all times.");
			if (HandleSettings.ReadSBool("Always_on_Top"))
			{
				base.Topmost = true;
				Always_on_Top.EnableSwitch();
			}
			else
			{
				base.Topmost = false;
				Always_on_Top.DisableSwitch();
			}
			Always_on_Top.SwitchActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				if (HandleSettings.ReadSBool("Always_on_Top"))
				{
					HandleSettings.SaveBool("Always_on_Top", false);
					Always_on_Top.DisableSwitch();
					this.Topmost = false;
					return;
				}
				HandleSettings.SaveBool("Always_on_Top", true);
				Always_on_Top.EnableSwitch();
				this.Topmost = true;
			};
			this.SettingsWrapper.Children.Add(Always_on_Top);
			this.ExecutorMainWindow.Opacity = HandleSettings.ReadSValue("Window_Transparency");
			SSlider Window_Transparency = new SSlider("Window_Transparency", "", "Window Transparency", "Make Vega X as see through as you want it to be.", "Transparency");
			Window_Transparency.Slider.ValueChanged += delegate(object s, RoutedPropertyChangedEventArgs<double> x)
			{
				HandleSettings.SaveValue("Window_Transparency", Window_Transparency.Slider.Value);
				this.ExecutorMainWindow.Opacity = Window_Transparency.Slider.Value;
			};
			this.SettingsWrapper.Children.Add(Window_Transparency);
			SSwitch ScriptBlox_ScriptHub_DefaultPosture = new SSwitch("", "Toggle Script Hub Default Page", "ScriptBlox will be set as the Default Page when this Setting is enabled.");
			if (HandleSettings.ReadSBool("ScriptBlox_ScriptHub_DefaultPosture"))
			{
				ScriptBlox_ScriptHub_DefaultPosture.EnableSwitch();
			}
			else
			{
				ScriptBlox_ScriptHub_DefaultPosture.DisableSwitch();
			}
			ScriptBlox_ScriptHub_DefaultPosture.SwitchActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				if (HandleSettings.ReadSBool("ScriptBlox_ScriptHub_DefaultPosture"))
				{
					HandleSettings.SaveBool("ScriptBlox_ScriptHub_DefaultPosture", false);
					ScriptBlox_ScriptHub_DefaultPosture.DisableSwitch();
					return;
				}
				HandleSettings.SaveBool("ScriptBlox_ScriptHub_DefaultPosture", true);
				ScriptBlox_ScriptHub_DefaultPosture.EnableSwitch();
			};
			this.SettingsWrapper.Children.Add(ScriptBlox_ScriptHub_DefaultPosture);
			SColorButton ChangeAccentColor = new SColorButton();
			ChangeAccentColor.ButtonActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				ColorDialog colorDialog = new ColorDialog();
				if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					HandleSettings.SaveString("Accent_Color", new SolidColorBrush(Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)).ToString());
					string str = HandleSettings.ReadSString("Accent_Color").Substring(3);
					ChangeAccentColor.ColorPreview.Background = (Brush)new BrushConverter().ConvertFromString("#7F" + str);
					ChangeAccentColor.ColorPreview.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
					this.LoadAccentColors();
				}
			};
			this.SettingsWrapper.Children.Add(ChangeAccentColor);
			SButton sbutton = new SButton("", "Remove Theme Image", "Theme Image suffers from the disappearance.");
			sbutton.ButtonActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				HandleSettings.SaveString("Image_Background", "");
				ImageBehavior.SetRepeatBehavior(this.ImageBg, new RepeatBehavior(TimeSpan.FromSeconds(0.0)));
				this.ImageBg.Visibility = Visibility.Collapsed;
				this.ExitBackground.CornerRadius = new CornerRadius(0.0, 5.0, 0.0, 0.0);
			};
			this.SettingsWrapper.Children.Add(sbutton);
			SSwitch Auto_Inject = new SSwitch("", "Auto Inject", "Inject Vega X automatically when the Game is open. (May contain bugs)");
			if (HandleSettings.ReadSBool("Auto_Inject"))
			{
				Auto_Inject.EnableSwitch();
			}
			else
			{
				this.AutoInject.Stop();
				Auto_Inject.DisableSwitch();
			}
			Auto_Inject.SwitchActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				if (HandleSettings.ReadSBool("Auto_Inject"))
				{
					HandleSettings.SaveBool("Auto_Inject", false);
					Auto_Inject.DisableSwitch();
					this.AutoInject.Stop();
					return;
				}
				HandleSettings.SaveBool("Auto_Inject", true);
				Auto_Inject.EnableSwitch();
				this.AutoInject.Start();
			};
			this.SettingsWrapper.Children.Add(Auto_Inject);
			SSwitch FPSUnlocker = new SSwitch("", "FPS Unlocker", "Unlock the Game FPS over 60fps, this makes the Game smoother.");
			if (File.Exists("C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Packages\\ROBLOXCORPORATION.ROBLOX_55nm5eh3cm0pr\\AC\\autoexec\\vegaxfpsunlocker.txt"))
			{
				FPSUnlocker.EnableSwitch();
			}
			else
			{
				FPSUnlocker.DisableSwitch();
			}
			FPSUnlocker.SwitchActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				if (File.Exists("C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Packages\\ROBLOXCORPORATION.ROBLOX_55nm5eh3cm0pr\\AC\\autoexec\\vegaxfpsunlocker.txt"))
				{
					try
					{
						File.Delete("C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Packages\\ROBLOXCORPORATION.ROBLOX_55nm5eh3cm0pr\\AC\\autoexec\\vegaxfpsunlocker.txt");
					}
					catch (Exception ex)
					{
						System.Windows.MessageBox.Show("Disabling FPS Unlocker Failed!\n" + ex.ToString(), "Vega X");
					}
					FPSUnlocker.DisableSwitch();
					return;
				}
				try
				{
					File.WriteAllText("C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Packages\\ROBLOXCORPORATION.ROBLOX_55nm5eh3cm0pr\\AC\\autoexec\\vegaxfpsunlocker.txt", "setfpscap(999)");
				}
				catch (Exception ex2)
				{
					System.Windows.MessageBox.Show("Enabling FPS Unlocker Failed!\n" + ex2.ToString(), "Vega X");
				}
				FPSUnlocker.EnableSwitch();
			};
			this.SettingsWrapper.Children.Add(FPSUnlocker);
			SSwitch SetClipboardSwitch = new SSwitch("clipboard", "Set Clipboard Function", "Required for Scripts with Key Systems. Changes made after reopen.");
			if (HandleSettings.ReadSBool("ToggleSetClipboard"))
			{
				SetClipboardSwitch.EnableSwitch();
			}
			else
			{
				SetClipboardSwitch.DisableSwitch();
			}
			SetClipboardSwitch.SwitchActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				if (HandleSettings.ReadSBool("ToggleSetClipboard"))
				{
					HandleSettings.SaveBool("ToggleSetClipboard", false);
					SetClipboardSwitch.DisableSwitch();
					return;
				}
				HandleSettings.SaveBool("ToggleSetClipboard", true);
				SetClipboardSwitch.EnableSwitch();
			};
			this.SettingsWrapper.Children.Add(SetClipboardSwitch);
			SButton sbutton2 = new SButton("", "Close Game Processes", "Closes all the Processes of the Game if the Game freezes.");
			sbutton2.ButtonActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				Process[] processesByName = Process.GetProcessesByName("Windows10Universal");
				for (int i = 0; i < processesByName.Length; i++)
				{
					processesByName[i].Kill();
				}
				System.Windows.MessageBox.Show("Killed all Game Processes.", "Vega X");
			};
			this.SettingsWrapper.Children.Add(sbutton2);
			SSwitch Check_Inject = new SSwitch("", "Check for Injection", "Check if Vega X has been Injected Successfully.");
			if (HandleSettings.ReadSBool("Check_Inject"))
			{
				Check_Inject.EnableSwitch();
			}
			else
			{
				this.InjectCheck.Stop();
				Check_Inject.DisableSwitch();
			}
			Check_Inject.SwitchActivator.Click += delegate(object s, RoutedEventArgs x)
			{
				if (HandleSettings.ReadSBool("Check_Inject"))
				{
					HandleSettings.SaveBool("Check_Inject", false);
					Check_Inject.DisableSwitch();
					this.InjectCheck.Stop();
					this.VegaXTitle.Content = this.VegaXTitleContent;
					return;
				}
				HandleSettings.SaveBool("Check_Inject", true);
				Check_Inject.EnableSwitch();
				this.InjectCheck.Start();
			};
			this.SettingsWrapper.Children.Add(Check_Inject);
		}

		void LoadImageBackground()
		{
			string text = HandleSettings.ReadSString("Image_Background");
			if (text != "" && File.Exists(text))
			{
				try
				{
					string extension = System.IO.Path.GetExtension(text);
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.BeginInit();
					bitmapImage.UriSource = new Uri(text);
					bitmapImage.EndInit();
					if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
					{
						HandleSettings.SaveString("Image_Background", text);
						ImageBehavior.SetAnimatedSource(this.ImageBg, bitmapImage);
					}
					else if (extension == ".gif")
					{
						HandleSettings.SaveString("Image_Background", text);
						ImageBehavior.SetAnimatedSource(this.ImageBg, bitmapImage);
						ImageBehavior.SetRepeatBehavior(this.ImageBg, RepeatBehavior.Forever);
					}
					else
					{
						HandleSettings.SaveString("Image_Background", "");
						System.Windows.MessageBox.Show("Invalid Image Extension, please convert the image to png, jpg, jpeg, or gif.", "Vega X");
					}
				}
				catch (Exception ex)
				{
					HandleSettings.SaveString("Image_Background", "");
					System.Windows.MessageBox.Show("The image you are using is incompatible with Vega X. It has been removed from your pleasure.\n\n" + ex.ToString(), "Vega X");
				}
			}
		}

		void LoadAccentColors()
		{
			string str = HandleSettings.ReadSString("Accent_Color").Substring(3);
			this.ScriptBloxWrapper.Children.Clear();
			this.LoadScriptBlox();
			this.SetFavoritePanel();
			this.border.Background = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
			this.ExitBackground.Background = (Brush)new BrushConverter().ConvertFromString("#7F" + str);
			this.ExitBackground.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
			this.InjectF.Background = (Brush)new BrushConverter().ConvertFromString("#7C" + str);
			this.InjectF.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
			this.ExecuteF.Background = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
			this.OpenFileF.Background = (Brush)new BrushConverter().ConvertFromString("#3F" + str);
			this.OpenFileF.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
			this.SaveFileF.Background = (Brush)new BrushConverter().ConvertFromString("#3F" + str);
			this.SaveFileF.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
			this.ClearTextBoxF.Background = (Brush)new BrushConverter().ConvertFromString("#3F" + str);
			this.ClearTextBoxF.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
			this.SettingsF.Background = (Brush)new BrushConverter().ConvertFromString("#3F" + str);
			this.SettingsF.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
			this.ScriptHubF.Background = (Brush)new BrushConverter().ConvertFromString("#3F" + str);
			this.ScriptHubF.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
		}

		void OpenProcess(string link)
		{
			try
			{
				Process.Start(link);
			}
			catch
			{
				System.Windows.Clipboard.SetText(link);
				System.Windows.MessageBox.Show("No Default Browser was found, so we have copied the link to your Clipboard.\nDo CTRL+V to paste the link into your preferred TextBox.", "Vega X");
			}
		}

		void EnableGrid(Grid SelectedGrid)
		{
			SelectedGrid.Margin = new Thickness(0.0, 260.0, 0.0, -180.0);
			SelectedGrid.Visibility = Visibility.Visible;
			this.Fade(SelectedGrid);
			this.ObjectShift(TimeSpan.FromMilliseconds(1000.0), SelectedGrid, SelectedGrid.Margin, new Thickness(0.0, 40.0, 0.0, 40.0));
		}

		void DisableGrid(Grid SelectedGrid)
		{
			Executor.<DisableGrid>d__29 <DisableGrid>d__;
			<DisableGrid>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisableGrid>d__.<>4__this = this;
			<DisableGrid>d__.SelectedGrid = SelectedGrid;
			<DisableGrid>d__.<>1__state = -1;
			<DisableGrid>d__.<>t__builder.Start<Executor.<DisableGrid>d__29>(ref <DisableGrid>d__);
		}

		string CleanString(string CleanableString)
		{
			return Regex.Replace(CleanableString, "[^\\u0000-\\u007F]+", "");
		}

		void LoadScriptBlox()
		{
			new Thread(delegate()
			{
				Executor.<<LoadScriptBlox>b__37_0>d <<LoadScriptBlox>b__37_0>d;
				<<LoadScriptBlox>b__37_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<LoadScriptBlox>b__37_0>d.<>4__this = this;
				<<LoadScriptBlox>b__37_0>d.<>1__state = -1;
				<<LoadScriptBlox>b__37_0>d.<>t__builder.Start<Executor.<<LoadScriptBlox>b__37_0>d>(ref <<LoadScriptBlox>b__37_0>d);
			}).Start();
		}

		void SetFavoritePanel()
		{
			new Thread(delegate()
			{
				base.Dispatcher.Invoke(delegate()
				{
					this.FavoriteScriptsWrapper.Children.Clear();
				});
				try
				{
					foreach (string path in Directory.EnumerateFiles("bin\\favoritedscripts\\", "*.json"))
					{
						JObject.Parse(File.ReadAllText(path))["Script"].ToList<JToken>().ForEach(delegate(JToken item)
						{
							RoutedEventHandler <>9__4;
							base.Dispatcher.Invoke(delegate()
							{
								if (item["Name"].ToString().ToLower().Contains(this.ScriptHubSearch.Text.ToLower()))
								{
									FavoritesSquare FavoritedScript = new FavoritesSquare();
									try
									{
										if (item["Image"].ToString().Contains("rbxcdn.com"))
										{
											FavoritedScript.ScriptImage.ImageSource = new BitmapImage(new Uri(item["Image"].ToString()));
										}
										else
										{
											FavoritedScript.ScriptImage.ImageSource = new BitmapImage(new Uri("https://scriptblox.com" + item["Image"].ToString()));
										}
									}
									catch
									{
									}
									FavoritedScript.ScriptName.Content = this.CleanString(item["Name"].ToString());
									FavoritedScript.GameName.Content = this.CleanString(item["GameName"].ToString());
									System.Windows.Controls.Primitives.ButtonBase executeScriptB = FavoritedScript.ExecuteScriptB;
									RoutedEventHandler value;
									if ((value = <>9__4) == null)
									{
										value = (<>9__4 = delegate(object s, RoutedEventArgs e)
										{
											new DLLInterfacing().Execute(item["Script"].ToString());
										});
									}
									executeScriptB.Click += value;
									FavoritedScript.RemoveScriptB.Click += delegate(object s, RoutedEventArgs e)
									{
										(FavoritedScript.Parent as WrapPanel).Children.Remove(FavoritedScript);
										File.Delete("bin\\favoritedscripts\\" + this.CleanString(item["Name"].ToString()) + ".json");
									};
									this.FavoriteScriptsWrapper.Children.Add(FavoritedScript);
								}
							});
						});
					}
				}
				catch
				{
				}
				GC.Collect(2, GCCollectionMode.Forced);
			}).Start();
		}

		void ScriptScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if ((this.ScriptScroller.ScrollableHeight < 0.0 || this.ScriptScroller.VerticalOffset == this.ScriptScroller.ScrollableHeight) && this.ScriptBloxWrapper.Children.Count >= 18 && this.CurrentPage < this.MaxPage && this.ScriptHubLoaded)
			{
				this.CurrentPage++;
				this.LoadScriptBlox();
			}
		}

		void ScriptBloxSearch()
		{
			this.Game_SearchText_Changed = true;
			this.Game_SearchText = this.ScriptHubSearch.Text;
			this.CurrentPage = 1;
			this.ScriptBloxWrapper.Children.Clear();
			this.LoadScriptBlox();
			this.SetFavoritePanel();
		}

		void ScriptHubSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.ScriptBloxSearch();
			}
		}

		void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			this.ScriptBloxSearch();
		}

		void GameWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.GameWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString(HandleSettings.ReadSString("Accent_Color"));
			this.FavoriteWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");
			this.ScriptScroller.Visibility = Visibility.Visible;
			this.FavoriteScriptScroller.Visibility = Visibility.Collapsed;
		}

		void FavoriteWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.SetFavoritePanel();
			this.GameWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#FFFFFFFF");
			this.FavoriteWindowButton.Foreground = (Brush)new BrushConverter().ConvertFromString("#FFFFAC37");
			this.ScriptScroller.Visibility = Visibility.Collapsed;
			this.FavoriteScriptScroller.Visibility = Visibility.Visible;
		}

		static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
		{
			int i = 0;
			while (i < VisualTreeHelper.GetChildrenCount(parent))
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				T result;
				if (child != null && child is T)
				{
					result = (T)((object)child);
				}
				else
				{
					T t = Executor.FindVisualChild<T>(child);
					if (t == null)
					{
						i++;
						continue;
					}
					result = t;
				}
				return result;
			}
			return default(T);
		}

		void CloseWindows()
		{
			this.tabEdit.Close();
		}

		TabItem MakeTab(string text = "", string title = "New Tab")
		{
			bool loaded = false;
			TextEditor textEditor = Executor.MakeEditor();
			textEditor.Text = text;
			XmlReader reader = new XmlTextReader(File.OpenRead("./bin/highlighter.xshd"));
			textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
			textEditor.TextArea.TextView.ElementGenerators.Add(new NoShittyTextLag());
			System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox
			{
				Text = title,
				IsHitTestVisible = false,
				IsEnabled = false,
				TextWrapping = TextWrapping.NoWrap,
				Style = (base.TryFindResource("InvisibleTextBox") as Style)
			};
			TabItem tab = new TabItem
			{
				Content = textEditor,
				AllowDrop = true,
				Style = (base.TryFindResource("Tab") as Style),
				Header = textBox
			};
			tab.Loaded += delegate(object sender, RoutedEventArgs e)
			{
				if (loaded)
				{
					return;
				}
				tab.GetTemplateChild("CloseButton").Click += delegate(object sender1, RoutedEventArgs e1)
				{
					this.TabCounter--;
					this.TabCtrl.Items.Remove(tab);
				};
				loaded = true;
			};
			tab.MouseDown += delegate(object sender, MouseButtonEventArgs e)
			{
				if (e.OriginalSource is Border)
				{
					if (e.MiddleButton == MouseButtonState.Pressed)
					{
						this.TabCtrl.Items.Remove(tab);
						return;
					}
					if (e.RightButton == MouseButtonState.Pressed)
					{
						this.tabEdit.Left = e.GetPosition(null).X - 12.0 + this.Left;
						this.tabEdit.Top = e.GetPosition(null).Y - 12.0 + this.Top;
						this.tabEdit.Show(tab);
					}
				}
			};
			string oldHeader = title;
			textBox.GotFocus += delegate(object sender, RoutedEventArgs e)
			{
				oldHeader = textBox.Text;
				textBox.CaretIndex = textBox.Text.Length - 1;
			};
			textBox.KeyDown += delegate(object s, System.Windows.Input.KeyEventArgs e)
			{
				if (textBox.Text == "")
				{
					Key key = e.Key;
					if (key != Key.Return)
					{
						if (key != Key.Escape)
						{
							return;
						}
						textBox.Text = oldHeader;
					}
					textBox.Text = "New Tab";
					System.Windows.MessageBox.Show("Tab name cannot be empty, so sorry!", "Vega X");
					textBox.IsEnabled = false;
					return;
				}
				Key key2 = e.Key;
				if (key2 != Key.Return)
				{
					if (key2 != Key.Escape)
					{
						return;
					}
					textBox.Text = oldHeader;
				}
				textBox.IsEnabled = false;
			};
			textBox.LostFocus += delegate(object sender, RoutedEventArgs e)
			{
				textBox.IsEnabled = false;
			};
			this.TabCtrl.SelectedIndex = this.TabCtrl.Items.Add(tab);
			return tab;
		}

		public static TextEditor MakeEditor()
		{
			TextEditor textEditor = new TextEditor();
			textEditor.ShowLineNumbers = true;
			textEditor.Background = new SolidColorBrush(Color.FromArgb(0, 25, 25, 25));
			textEditor.BorderBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));
			textEditor.Foreground = new SolidColorBrush(Color.FromRgb(235, 235, 235));
			textEditor.LineNumbersForeground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
			textEditor.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
			textEditor.FontFamily = new FontFamily("Consolas");
			textEditor.Options.EnableEmailHyperlinks = false;
			textEditor.Options.EnableHyperlinks = false;
			textEditor.Options.AllowScrollBelowDocument = true;
			textEditor.ShowLineNumbers = true;
			XmlTextReader reader = new XmlTextReader(File.OpenRead("./bin/highlighter.xshd"));
			textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
			return textEditor;
		}

		public void RefreshScriptList()
		{
			Executor.<RefreshScriptList>d__50 <RefreshScriptList>d__;
			<RefreshScriptList>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RefreshScriptList>d__.<>4__this = this;
			<RefreshScriptList>d__.<>1__state = -1;
			<RefreshScriptList>d__.<>t__builder.Start<Executor.<RefreshScriptList>d__50>(ref <RefreshScriptList>d__);
		}

		public void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
		{
			base.Dispatcher.Invoke(delegate()
			{
				this.RefreshScriptList();
			});
		}

		public void FileSystemWatcher_Renamed(object sender, FileSystemEventArgs e)
		{
			base.Dispatcher.Invoke(delegate()
			{
				this.RefreshScriptList();
			});
		}

		public void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			base.Dispatcher.Invoke(delegate()
			{
				this.RefreshScriptList();
			});
		}

		void ListBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.RefreshScriptList();
		}

		void ScriptList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (this.ScriptList.SelectedIndex != -1)
				{
					string str = ".\\add_scripts_here\\";
					object selectedItem = this.ScriptList.SelectedItem;
					if (this.TabCtrl.Items.Count != 0)
					{
						this.TabCtrl.Dispatcher.BeginInvoke(new Action(delegate()
						{
							Executor.FindVisualChild<TextEditor>(this.TabCtrl).Text = File.ReadAllText(str + ((selectedItem != null) ? selectedItem.ToString() : null));
						}), Array.Empty<object>());
					}
				}
			}
			catch
			{
			}
		}

		void InjectB_Click(object sender, RoutedEventArgs e)
		{
			FluxInteractionsAPI.inject();
		}

		void ExecuteB_Click(object sender, RoutedEventArgs e)
		{
			TextEditor textEditor = Executor.FindVisualChild<TextEditor>(this.TabCtrl);
			if (textEditor != null)
			{
				new DLLInterfacing().Execute(textEditor.Text);
			}
		}

		void OpenFileB_Click(object sender, RoutedEventArgs e)
		{
			TextEditor textEditor = Executor.FindVisualChild<TextEditor>(this.TabCtrl);
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "All files (*.*)|*.*";
			ofd.Title = "Vega X | Open File";
			ofd.InitialDirectory = System.Windows.Forms.Application.StartupPath;
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.TabCtrl.Dispatcher.BeginInvoke(new Action(delegate()
				{
					textEditor.Text = File.ReadAllText(ofd.FileName);
				}), Array.Empty<object>());
			}
		}

		void SaveFileB_Click(object sender, RoutedEventArgs e)
		{
			TextEditor textEditor = Executor.FindVisualChild<TextEditor>(this.TabCtrl);
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Text File (*.txt)|*.txt|Lua Scripts (*.lua)|*.lua";
			sfd.Title = "Vega X | Save File";
			sfd.InitialDirectory = System.Windows.Forms.Application.StartupPath;
			this.TabCtrl.Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					File.WriteAllText(sfd.FileName, textEditor.Text);
				}
			}), Array.Empty<object>());
		}

		void ClearTextBoxB_Click(object sender, RoutedEventArgs e)
		{
			Executor.FindVisualChild<TextEditor>(this.TabCtrl).Text = "";
		}

		void SettingsB_Click(object sender, RoutedEventArgs e)
		{
			Executor.<SettingsB_Click>d__63 <SettingsB_Click>d__;
			<SettingsB_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SettingsB_Click>d__.<>4__this = this;
			<SettingsB_Click>d__.<>1__state = -1;
			<SettingsB_Click>d__.<>t__builder.Start<Executor.<SettingsB_Click>d__63>(ref <SettingsB_Click>d__);
		}

		void ScriptHubB_Click(object sender, RoutedEventArgs e)
		{
			Executor.<ScriptHubB_Click>d__64 <ScriptHubB_Click>d__;
			<ScriptHubB_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ScriptHubB_Click>d__.<>4__this = this;
			<ScriptHubB_Click>d__.<>1__state = -1;
			<ScriptHubB_Click>d__.<>t__builder.Start<Executor.<ScriptHubB_Click>d__64>(ref <ScriptHubB_Click>d__);
		}

		static string current_ui_ver = "0.8";

		string VegaXTitleContent = "Vega X : 3." + Executor.current_ui_ver;

		DispatcherTimer AutoInject;

		DispatcherTimer InjectCheck;

		int TabCounter;

		HttpClient httpclient = new HttpClient();

		Storyboard StoryBoard = new Storyboard();

		TimeSpan duration = TimeSpan.FromMilliseconds(500.0);

		TimeSpan duration2 = TimeSpan.FromMilliseconds(1000.0);

		bool ScriptHubLoaded;

		string Game_SearchText = "a";

		bool Game_SearchText_Changed = true;

		int CurrentPage = 1;

		int MaxPage = 500;

		readonly TabEdit tabEdit;

		bool SettingsOpen;

		bool ScriptHubOpen;

		public class jsonstrings
		{
			public List<Executor.jsonstrings.ScriptHub> Script;

			public class ScriptHub
			{
				public string Image { get; set; }

				public string Name { get; set; }

				public string GameName { get; set; }

				public string Script { get; set; }
			}
		}
	}
}
