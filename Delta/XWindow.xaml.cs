using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Delta
{
	public partial class XWindow : Window, IStyleConnector
	{
		public XWindow()
		{
			this.InitializeComponent();
			this.setupdelta();
		}

		static string CalculateSHA256(string Path)
		{
			string result;
			using (SHA384 sha = SHA384.Create())
			{
				using (FileStream fileStream = File.OpenRead(Path))
				{
					byte[] array = sha.ComputeHash(fileStream);
					StringBuilder stringBuilder = new StringBuilder();
					foreach (byte b in array)
					{
						stringBuilder.Append(b.ToString("x2"));
					}
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		void Inject()
		{
			Process[] processesByName = Process.GetProcessesByName("Windows10Universal");
			if (processesByName.Length == 0)
			{
				MessageBox.Show("Please open roblox from the microsoft store before injecting", "Delta - Injection Error");
				return;
			}
			string b = XWindow.CalculateSHA256(processesByName[0].MainModule.FileName);
			foreach (module module in this.current_modules.modules)
			{
				if (module.hash == b)
				{
					this.CurrentModule = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + module.name;
				}
			}
			if (this.CurrentModule == "")
			{
				MessageBox.Show("You're using a version of roblox which delta does not support yet. Please wait for us to update!", "Delta - Injection Error");
				return;
			}
			if (!File.Exists(this.CurrentModule))
			{
				MessageBox.Show("It seems like the Delta-DLL was deleted, which most likely happened because of your anti-virus. Please turn it off and redownload Delta", "Delta");
				return;
			}
			switch (API.r_inject(this.CurrentModule))
			{
			case API.Result.Success:
				MessageBox.Show("Successfully attached to Roblox!", "Delta - Injection Result");
				this._api.create_links();
				this._api.setup_ipc();
				return;
			case API.Result.DLLNotFound:
				MessageBox.Show("The specified DLL could not be found. Please ensure that the DLL exists and that the path is correct.", "Delta - Injection Error");
				return;
			case API.Result.OpenProcFail:
				MessageBox.Show("Failed to open the target process.", "Delta - Injection Error");
				return;
			case API.Result.AllocFail:
				MessageBox.Show("Failed to allocate memory in the target process.", "Delta - Injection Error");
				return;
			case API.Result.LoadLibFail:
				MessageBox.Show("Failed to load the specified DLL into the target process. Please ensure that the DLL is valid and that it can be loaded into the target process.", "Delta - Injection Error");
				return;
			case API.Result.AlreadyInjected:
				MessageBox.Show("The specified DLL is already injected into the target process.", "Delta - Injection Error");
				return;
			case API.Result.ProcNotOpen:
				MessageBox.Show("Roblox from the Microsoft Store is not currently open. Please make sure that you have it open before you inject!", "Delta - Injection Error");
				return;
			case API.Result.Unknown:
				MessageBox.Show("An unknown error occurred. Please try again or contact support for assistance.", "Delta - Injection Error");
				return;
			default:
				return;
			}
		}

		public void RunScript(string Script)
		{
			if (this.CurrentModule == "")
			{
				MessageBox.Show("The specified DLL is not attached to the target process.", "Delta - Injection Error");
				return;
			}
			API.run_script(this.CurrentModule, Script);
		}

		public void setupdelta()
		{
			XWindow.<setupdelta>d__18 <setupdelta>d__;
			<setupdelta>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<setupdelta>d__.<>4__this = this;
			<setupdelta>d__.<>1__state = -1;
			<setupdelta>d__.<>t__builder.Start<XWindow.<setupdelta>d__18>(ref <setupdelta>d__);
		}

		public void startup()
		{
			WebClient webClient = new WebClient();
			this.current_modules = JsonConvert.DeserializeObject<DLls>(File.ReadAllText(System.IO.Path.Combine(Environment.CurrentDirectory, "bin\\modules.json")));
			foreach (module module in this.current_modules.modules)
			{
				bool flag = File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + module.name);
				if (module.active && !flag)
				{
					webClient.DownloadFile(module.online_link, AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + module.name);
				}
			}
			webClient.Dispose();
		}

		static bool IsValidJson(string strInput)
		{
			if (string.IsNullOrWhiteSpace(strInput))
			{
				return false;
			}
			strInput = strInput.Trim();
			if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || (strInput.StartsWith("[") && strInput.EndsWith("]")))
			{
				try
				{
					JToken.Parse(strInput);
					return true;
				}
				catch (JsonReaderException ex)
				{
					Console.WriteLine(ex.Message);
					return false;
				}
				catch (Exception ex2)
				{
					Console.WriteLine(ex2.ToString());
					return false;
				}
				return false;
			}
			return false;
		}

		public void show_eula()
		{
			XWindow.<show_eula>d__22 <show_eula>d__;
			<show_eula>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<show_eula>d__.<>4__this = this;
			<show_eula>d__.<>1__state = -1;
			<show_eula>d__.<>t__builder.Start<XWindow.<show_eula>d__22>(ref <show_eula>d__);
		}

		public void setup_themes()
		{
			if (this.theme_wrap_panel.Children.Count == 0)
			{
				using (WebClient webClient = new WebClient())
				{
					foreach (Theme this_theme in JsonConvert.DeserializeObject<Root>(webClient.DownloadString("https://gitlab.com/littlekiller2927/deltacore/-/raw/main/deltathemes.json")).themes)
					{
						this.theme_wrap_panel.Children.Add(new ThemeItem(this_theme));
					}
				}
			}
		}

		void LoadSettings()
		{
			if (!File.Exists(System.IO.Path.Combine(this.deltacore, "Settings.json")) || !File.ReadAllText(System.IO.Path.Combine(this.deltacore, "Settings.json")).Contains("thai_thai") || !XWindow.IsValidJson(File.ReadAllText(System.IO.Path.Combine(this.deltacore, "Settings.json"))))
			{
				File.WriteAllText(System.IO.Path.Combine(this.deltacore, "Settings.json"), "[  {    \"CBName\": \"opacity\",    \"CBState\": false  },  {    \"CBName\": \"topmost\",    \"CBState\": false  },  {    \"CBName\": \"delta\",    \"CBState\": true  },  {    \"CBName\": \"wrd\",    \"CBState\": false  },  {    \"CBName\": \"autoinj\",    \"CBState\": false  },  {    \"CBName\": \"verscheck\",    \"CBState\": false  },  {    \"CBName\": \"autofade\",    \"CBState\": false  },  {    \"CBName\": \"fpsunlocker\",    \"CBState\": false  },  {    \"CBName\": \"eng_eng\",    \"CBState\": true  },  {    \"CBName\": \"ger_deutsch\",    \"CBState\": false  },  {    \"CBName\": \"tur_tur\",    \"CBState\": false  },  {    \"CBName\": \"spa_esp\",    \"CBState\": false  },  {    \"CBName\": \"ind_bah\",    \"CBState\": false  },  {    \"CBName\": \"por_por\",    \"CBState\": false  },  {    \"CBName\": \"fil_pil\",    \"CBState\": false  },  {    \"CBName\": \"pol_pol\",    \"CBState\": false  },  {    \"CBName\": \"fin_sou\",    \"CBState\": false  },  {    \"CBName\": \"fre_fra\",    \"CBState\": false  },  {    \"CBName\": \"thai_thai\",    \"CBState\": false  }]");
			}
			object arg = JsonConvert.DeserializeObject(File.ReadAllText(System.IO.Path.Combine(this.deltacore, "Settings.json")));
			if (XWindow.<>o__24.<>p__7 == null)
			{
				XWindow.<>o__24.<>p__7 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(XWindow)));
			}
			foreach (object arg2 in XWindow.<>o__24.<>p__7.Target(XWindow.<>o__24.<>p__7, arg))
			{
				if (XWindow.<>o__24.<>p__2 == null)
				{
					XWindow.<>o__24.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(XWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, object, bool> target = XWindow.<>o__24.<>p__2.Target;
				CallSite <>p__ = XWindow.<>o__24.<>p__2;
				if (XWindow.<>o__24.<>p__1 == null)
				{
					XWindow.<>o__24.<>p__1 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(XWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, object, bool, object> target2 = XWindow.<>o__24.<>p__1.Target;
				CallSite <>p__2 = XWindow.<>o__24.<>p__1;
				if (XWindow.<>o__24.<>p__0 == null)
				{
					XWindow.<>o__24.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(XWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				if (target(<>p__, target2(<>p__2, XWindow.<>o__24.<>p__0.Target(XWindow.<>o__24.<>p__0, arg2, "CBState"), true)))
				{
					if (XWindow.<>o__24.<>p__6 == null)
					{
						XWindow.<>o__24.<>p__6 = CallSite<Func<CallSite, object, CheckBox>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CheckBox), typeof(XWindow)));
					}
					Func<CallSite, object, CheckBox> target3 = XWindow.<>o__24.<>p__6.Target;
					CallSite <>p__3 = XWindow.<>o__24.<>p__6;
					if (XWindow.<>o__24.<>p__5 == null)
					{
						XWindow.<>o__24.<>p__5 = CallSite<Func<CallSite, XWindow, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "FindName", null, typeof(XWindow), new CSharpArgumentInfo[]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
						}));
					}
					Func<CallSite, XWindow, object, object> target4 = XWindow.<>o__24.<>p__5.Target;
					CallSite <>p__4 = XWindow.<>o__24.<>p__5;
					if (XWindow.<>o__24.<>p__4 == null)
					{
						XWindow.<>o__24.<>p__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(XWindow), new CSharpArgumentInfo[]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
						}));
					}
					Func<CallSite, object, object> target5 = XWindow.<>o__24.<>p__4.Target;
					CallSite <>p__5 = XWindow.<>o__24.<>p__4;
					if (XWindow.<>o__24.<>p__3 == null)
					{
						XWindow.<>o__24.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(XWindow), new CSharpArgumentInfo[]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
						}));
					}
					target3(<>p__3, target4(<>p__4, this, target5(<>p__5, XWindow.<>o__24.<>p__3.Target(XWindow.<>o__24.<>p__3, arg2, "CBName")))).IsChecked = new bool?(true);
				}
			}
		}

		public void search_scriptblox(string query = "pet sim x", string filters = "hot")
		{
			XWindow.<search_scriptblox>d__25 <search_scriptblox>d__;
			<search_scriptblox>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<search_scriptblox>d__.<>4__this = this;
			<search_scriptblox>d__.query = query;
			<search_scriptblox>d__.filters = filters;
			<search_scriptblox>d__.<>1__state = -1;
			<search_scriptblox>d__.<>t__builder.Start<XWindow.<search_scriptblox>d__25>(ref <search_scriptblox>d__);
		}

		IEasingFunction EasingSmooth { get; set; } = new QuarticEase
		{
			EasingMode = EasingMode.EaseOut
		};

		IEasingFunction Smooth { get; set; } = new QuarticEase
		{
			EasingMode = EasingMode.EaseInOut
		};

		public void heightanim(DependencyObject Element, double one, double two)
		{
			Storyboard storyboard = new Storyboard();
			DoubleAnimation doubleAnimation = new DoubleAnimation(one, two, TimeSpan.Parse("00:00:1"));
			doubleAnimation.EasingFunction = this.Smooth;
			Storyboard.SetTarget(storyboard, Element);
			Storyboard.SetTargetProperty(storyboard, new PropertyPath(FrameworkElement.HeightProperty));
			storyboard.Children.Add(doubleAnimation);
			storyboard.Begin();
		}

		public void opacityanim(DependencyObject Element, double one, double two)
		{
			Storyboard storyboard = new Storyboard();
			DoubleAnimation doubleAnimation = new DoubleAnimation(one, two, TimeSpan.Parse("00:00:1"));
			doubleAnimation.EasingFunction = this.Smooth;
			Storyboard.SetTarget(storyboard, Element);
			Storyboard.SetTargetProperty(storyboard, new PropertyPath(UIElement.OpacityProperty));
			storyboard.Children.Add(doubleAnimation);
			storyboard.Begin();
		}

		public void widthanim(DependencyObject Element, double one, double two, IEasingFunction idk)
		{
			Storyboard storyboard = new Storyboard();
			DoubleAnimation doubleAnimation = new DoubleAnimation(one, two, TimeSpan.Parse("00:00:1"));
			doubleAnimation.EasingFunction = idk;
			Storyboard.SetTarget(storyboard, Element);
			Storyboard.SetTargetProperty(storyboard, new PropertyPath(FrameworkElement.WidthProperty));
			storyboard.Children.Add(doubleAnimation);
			storyboard.Begin();
		}

		public void marginanim(DependencyObject Element, Thickness one, Thickness two, IEasingFunction idk)
		{
			Storyboard storyboard = new Storyboard();
			ThicknessAnimation thicknessAnimation = new ThicknessAnimation(one, two, TimeSpan.Parse("00:00:1"));
			thicknessAnimation.EasingFunction = idk;
			Storyboard.SetTarget(storyboard, Element);
			Storyboard.SetTargetProperty(storyboard, new PropertyPath(FrameworkElement.MarginProperty));
			storyboard.Children.Add(thicknessAnimation);
			storyboard.Begin();
		}

		public void create_a_new_tab(string tabname = "NewScript.lua", string content = "")
		{
			this.tabControl1.Items.Add(this.CreateTab(tabname, ""));
			using (Stream stream = File.OpenRead(Directory.GetCurrentDirectory() + "\\bin\\lua.xshd"))
			{
				using (XmlTextReader xmlTextReader = new XmlTextReader(stream))
				{
					(this.tabControl1.SelectedContent as TextEditor).SyntaxHighlighting = HighlightingLoader.Load(xmlTextReader, HighlightingManager.Instance);
				}
			}
			(this.tabControl1.SelectedContent as TextEditor).TextArea.TextView.LinkTextUnderline = false;
			(this.tabControl1.SelectedContent as TextEditor).TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush(Color.FromRgb(63, 139, 200));
			this.dolines();
			this.SetText(content);
		}

		public TabItem GetCurrentTab()
		{
			if (this.tabControl.SelectedIndex == -1)
			{
				return null;
			}
			return this.tabControl1.SelectedItem as TabItem;
		}

		public TabItem CreateTab(string Header, string Text = "")
		{
			TextBox header = new TextBox
			{
				Text = Header,
				IsEnabled = false,
				TextWrapping = TextWrapping.NoWrap,
				IsHitTestVisible = false,
				Style = (base.TryFindResource("InvisibleTextBox") as Style)
			};
			TabItem tabItem = new TabItem();
			tabItem.Header = header;
			tabItem.FontSize = 11.0;
			tabItem.Foreground = new SolidColorBrush(Color.FromRgb(86, 154, 200));
			tabItem.Style = (base.TryFindResource("tab") as Style);
			tabItem.IsSelected = true;
			tabItem.Content = this.AvalonEdit(Text);
			tabItem.MouseDown += delegate(object sender, MouseButtonEventArgs e)
			{
				if (e.OriginalSource is Border && e.RightButton == MouseButtonState.Pressed)
				{
					TextBox textBox = this.GetCurrentTab().Header as TextBox;
					textBox.IsEnabled = true;
					textBox.Focus();
					textBox.SelectAll();
				}
			};
			return tabItem;
		}

		public TextEditor AvalonEdit(string Text)
		{
			return new TextEditor
			{
				FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Assets/Fonts/#Jetbrains Mono"),
				ShowLineNumbers = true,
				FontSize = 12.0,
				Text = Text,
				Background = Brushes.Transparent,
				Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
				HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
		}

		void dolines()
		{
			(this.tabControl1.SelectedContent as TextEditor).TextArea.TextView.ElementGenerators.Add(new AvalonLines());
		}

		string GetText()
		{
			return (this.tabControl1.SelectedContent as TextEditor).Text;
		}

		void SetText(string Text = "")
		{
			(this.tabControl1.SelectedContent as TextEditor).Text = Text;
		}

		void button_Copy_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		void button_Copy1_Click(object sender, RoutedEventArgs e)
		{
			if (base.WindowState != WindowState.Maximized)
			{
				base.WindowState = WindowState.Maximized;
				return;
			}
			base.WindowState = WindowState.Normal;
		}

		void button_Copy2_Click(object sender, RoutedEventArgs e)
		{
			base.WindowState = WindowState.Minimized;
		}

		public void refresh_scriptbox(string query = "")
		{
			this.lbol.Items.Clear();
			string text = ".\\scripts";
			foreach (FileInfo fileInfo in new DirectoryInfo(text).GetFiles())
			{
				if (this.luaselected.IsChecked.Value)
				{
					if (fileInfo.Extension == ".lua" && fileInfo.Name.Contains(query))
					{
						this.lbol.Items.Add(new ListBoxItem
						{
							Content = fileInfo.Name,
							Tag = fileInfo.FullName,
							Style = (base.TryFindResource("listboxitem") as Style)
						});
					}
				}
				else if (this.txtselected.IsChecked.Value)
				{
					if (fileInfo.Extension == ".txt" && fileInfo.Name.Contains(query))
					{
						this.lbol.Items.Add(new ListBoxItem
						{
							Content = fileInfo.Name,
							Tag = fileInfo.FullName,
							Style = (base.TryFindResource("listboxitem") as Style)
						});
					}
				}
				else if (this.allselected.IsChecked.Value && fileInfo.Name.Contains(query))
				{
					this.lbol.Items.Add(new ListBoxItem
					{
						Content = fileInfo.Name,
						Tag = fileInfo.FullName,
						Style = (base.TryFindResource("listboxitem") as Style)
					});
				}
			}
			DirectoryInfo[] directories = new DirectoryInfo(text).GetDirectories("*.*", SearchOption.AllDirectories);
			for (int i = 0; i < directories.Length; i++)
			{
				foreach (FileInfo fileInfo2 in new DirectoryInfo(directories[i].FullName).GetFiles())
				{
					if (this.luaselected.IsChecked.Value)
					{
						if (fileInfo2.Extension == ".lua" && fileInfo2.Name.Contains(query))
						{
							this.lbol.Items.Add(new ListBoxItem
							{
								Content = fileInfo2.Name,
								Tag = fileInfo2.FullName,
								Style = (base.TryFindResource("listboxitem") as Style)
							});
						}
					}
					else if (this.txtselected.IsChecked.Value)
					{
						if (fileInfo2.Extension == ".txt" && fileInfo2.Name.Contains(query))
						{
							this.lbol.Items.Add(new ListBoxItem
							{
								Content = fileInfo2.Name,
								Tag = fileInfo2.FullName,
								Style = (base.TryFindResource("listboxitem") as Style)
							});
						}
					}
					else if (this.allselected.IsChecked.Value && fileInfo2.Name.Contains(query))
					{
						this.lbol.Items.Add(new ListBoxItem
						{
							Content = fileInfo2.Name,
							Tag = fileInfo2.FullName,
							Style = (base.TryFindResource("listboxitem") as Style)
						});
					}
				}
			}
		}

		void lbol_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (this.lbol.Items.Count > 0)
				{
					ListBoxItem listBoxItem = this.lbol.SelectedItem as ListBoxItem;
					string text = File.ReadAllText(string.Format("{0}", listBoxItem.Tag));
					this.create_a_new_tab(string.Format("{0}", listBoxItem.Content), "");
					this.SetText(text);
				}
			}
			catch
			{
			}
		}

		public void uncheckalllangs(object sender)
		{
			foreach (object obj in this.etc1.Children)
			{
				CheckBox checkBox = (CheckBox)obj;
				if (checkBox != sender)
				{
					checkBox.IsChecked = new bool?(false);
				}
			}
		}

		public void englishlang()
		{
			this.header1.Text = "Auto Execute";
			this.header_Copy1.Text = "This automatically executes all scripts you want everytime you inject/serverhop";
			this.header9.Text = "Open DLL Folder";
			this.header_Copy10.Text = "This opens Delta's DLL Folder, that includes the workspace, logs and autoexec folder";
			this.header2.Text = "FPS Unlocker";
			this.header_Copy2.Text = "Unlock your frames per second to get a much more enhanced experience on Roblox";
			this.header3.Text = "Kill Roblox";
			this.header_Copy3.Text = "This kills/closes all currently running Roblox processes";
			this.header4.Text = "Opacity";
			this.header_Copy5.Text = "Makes Delta transparent by lowering the window opacity";
			this.header5.Text = "TopMost";
			this.header_Copy6.Text = "Makes the Delta UI stay focused on top of your other windows";
			this.header6.Text = "Auto Fade";
			this.header_Copy7.Text = "Automatically lower opacity upon window focus being lost";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "Use the powerful level 7 Delta API (recommended)";
			this.header13.Text = "WRD API";
			this.header_Copy14.Text = "Use the WeAreDevs API (level 7)";
			this.header8.Text = "Auto Inject";
			this.header_Copy9.Text = "This automatically injects for you everytime you open roblox";
			this.header7.Text = "Version Checking";
			this.header_Copy8.Text = "Corrects the roblox version if its wrong (Could slow injection)";
			this.header20.Text = "Reinstall DLL";
			this.header_Copy21.Text = "This option reinstalls Delta's DLL/Module which runs scripts";
			this.header21.Text = "Install Dependencies";
			this.header_Copy22.Text = "Reinstall VC Redist x64, x86 and DirectX automatically";
			this.no_results = "Sorry, We couldnt find any results :(";
			this.save_text = "Your settings were saved and will automatically load the next time you launch Delta!";
			this.auto_exec_prompt = "This can not be turned on/off because Delta automatically executes all files you put in the \"autoexec\" folder.\nDo you want to open the \"autoexec\" folder?";
			this.exec_btn.Content = "EXECUTE";
			this.clr_btn.Content = "CLEAR";
			this.opn_btn.Content = "OPEN";
			this.sve_btn.Content = "SAVE";
			this.inj_btn.Content = "INJECT";
			this.execute_again_button.Content = "Execute";
			this.browse_lbl.Text = "Browse";
			this.hot.Content = "Hot";
			this.old.Content = "Oldest";
			this.mostv.Content = "Most Viewed";
			this.FAVS.Content = "Favorites";
			this.desctxt.Text = "Description";
			this.search_scripts_folder_box.Tag = "Filter Items..";
			this.searchbox.Tag = "Filter Items..";
			this.allselected.Content = "ALL FILES";
			this.placeholdertext.Text = "We have over 4,000+ Scripts that you can search for! Start by doing a search :D";
			this.theme_title.Text = "Select a Theme!";
			this.theme_desc.Text = "Select a Theme to start! Apply it using the \"Use Theme\" Button";
			this.resettheme.Content = "Reset Theme";
			this.usetheme.Content = "Use Theme";
			this.homelbl.Text = "HOME";
			this.scrptslbl.Text = "SCRIPTS";
			this.thmlbl.Text = "THEMES";
			this.cnfglbl.Text = "CONFIG";
			this.header_Copy4.Text = "Configuration";
			this.saveset.Content = "Save Settings";
		}

		public void finnishlang()
		{
			this.header1.Text = "Automaattisesti suorita";
			this.header_Copy1.Text = "Tämä automaattisest suorittaa kaikki skriptit joka kerta kun sinä kiinnität Deltan peliisi/vaihdat serveriä";
			this.header9.Text = "Avaa DLL kansio";
			this.header_Copy10.Text = "Tämä avaa kaikki Deltan DLL kansiot, jotka sisältävät Työpaikan tallenettuihin skripteihisi ja autoexec kansiot";
			this.header2.Text = "FPS:n avaaja";
			this.header_Copy2.Text = "Tekee sinun Ruudunpäivitysnopeudesta isomman jotta saisit paljon paremman kokemuksen Robloxista";
			this.header3.Text = "Sulje Roblox";
			this.header_Copy3.Text = "Tämä tuhoaa/sulkee kaikki käynnissä olevat Roblox-prosessit";
			this.header4.Text = "Peittävyys";
			this.header_Copy5.Text = "Tekee Deltasta läpinäkyvän alentamalla ikkunan peittävyyttä ";
			this.header5.Text = "TopMost";
			this.header_Copy6.Text = "Saa Delta-käyttöliittymän keskittymään muiden ikkunojesi päälle";
			this.header6.Text = "Auto Fade";
			this.header_Copy7.Text = "Pienennä läpikuultamattomuutta automaattisesti, kun ikkunan tarkennus katoaa";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "Käytä voimakasta tason 7 Deltan Ohjelmointirajapintaa (suositeltu)";
			this.header13.Text = "WRD Ohjelmointirajapinta";
			this.header_Copy14.Text = "Käytä WeAreDevs:in Ohjelmointirajapintaa (taso 7)";
			this.header8.Text = "Automaattisesti suorita";
			this.header_Copy9.Text = "Tämä automaattisesti suorittaa skriptisi sinulle joka kerta kun avaat robloxin";
			this.header7.Text = "Version Tarkistus";
			this.header_Copy8.Text = "Korjaa/päivittää robloxin version Jos se on väärin (Voi hidastaa suoritus nopeutta)";
			this.header20.Text = "Lataa uudelleen DLL:ät";
			this.header_Copy21.Text = "Tämä vaihtoehto asentaa Deltan DLL/moduulin uudelleen, joka suorittaa komentosarjoja/skriptisi";
			this.header21.Text = "Asenna riippuvuudet";
			this.header_Copy22.Text = "Asenna VC Redist x64, x86 ja DirectX automaattisesti uudelleen";
			this.no_results = "Anteeki, emme löytäneet yhtään tuloksia :C :(";
			this.save_text = "Asetuksesi tallennettiin ja latautuvat automaattisesti, kun seuraavan kerran käynnistät Deltan!";
			this.auto_exec_prompt = "Tätä ei voi kytkeä päälle tai pois päältä, koska Delta suorittaa automaattisesti kaikki \"autoexec\"-kansioon lisäämäsi tiedostot.\nHaluatko avata \"autoexec\"-kansion?";
			this.exec_btn.Content = "SUORITA";
			this.clr_btn.Content = "TYHJENNÄ";
			this.opn_btn.Content = "AVAA";
			this.sve_btn.Content = "TALLENNA";
			this.inj_btn.Content = "SUORITA INJEKTIO";
			this.execute_again_button.Content = "Suorita";
			this.browse_lbl.Text = "Hae";
			this.hot.Content = "Kuuminta";
			this.old.Content = "Vanhin";
			this.mostv.Content = "Eniten katsottuin";
			this.FAVS.Content = "Suosikit";
			this.desctxt.Text = "Kuvaus";
			this.search_scripts_folder_box.Tag = "Filter Items..";
			this.searchbox.Tag = "Suodata kohteet..";
			this.allselected.Content = "KAIKKI TIEDOSTOT";
			this.placeholdertext.Text = "Meillä on yli 4 000 komentosarjaa, joita voit etsiä! Aloita tekemällä haku c:";
			this.theme_title.Text = "Valitse teema!";
			this.theme_desc.Text = "Aloita valitsemalla teema! Käytä sitä käyttämällä \"Use Theme\" painiketta";
			this.resettheme.Content = "Resetoi teema";
			this.usetheme.Content = "Käytä teema";
			this.homelbl.Text = "KOTI";
			this.scrptslbl.Text = "SKRIPTIT";
			this.thmlbl.Text = "TEEMAT";
			this.cnfglbl.Text = "ASETUKSIA";
			this.header_Copy4.Text = "Kokoonpano";
			this.saveset.Content = "Tallenna asetukset";
		}

		public void frenchlang()
		{
			this.header1.Text = "Éxecution Automatique";
			this.header_Copy1.Text = "Ceci éxecute automatiquement tous les scripts lorsque tu injectes";
			this.header9.Text = "Ouvrir le dossier DLL";
			this.header_Copy10.Text = "Ceci ouvre le dossier Delta, qui inclut l'espace de travail, les patchs de màj et l'éxecution automatique";
			this.header2.Text = "FPS Unlocker";
			this.header_Copy2.Text = "Débloquez vos FPS pour obtenir une expérience Roblox bien meilleure et fluide";
			this.header3.Text = "Arrêter Roblox";
			this.header_Copy3.Text = "Cela ferme tous les processus Roblox en cours";
			this.header4.Text = "Opacité";
			this.header_Copy5.Text = "Rend Delta transparent en réduisant l'opacité de la fenêtre";
			this.header5.Text = "PlusPopulaires";
			this.header_Copy6.Text = "Permet à l'interface utilisateur Delta de rester concentré sur vos autres fenêtres";
			this.header6.Text = "Fondu Automatique";
			this.header_Copy7.Text = "Réduction automatique de l'opacité lorsque le focus de la fenêtre est perdu";
			this.header12.Text = "Application Delta";
			this.header_Copy13.Text = "Utilisez la brillante application Delta au niveau 7 ! (recommandé)";
			this.header13.Text = "Application WRD";
			this.header_Copy14.Text = "Utilisez l'application WeAreDevs (Niveau 7)";
			this.header8.Text = "Injection Automatique";
			this.header_Copy9.Text = "Cela s'injecte automatiquement pour toi à chaque fois que tu lances Roblox !";
			this.header7.Text = "Check de la version";
			this.header_Copy8.Text = "Corrigez la version Roblox si ça ne fonctionne pas (Peut ralentir l'injection)";
			this.header20.Text = "Réinstallez le/les fichier/s DLL";
			this.header_Copy21.Text = "Cette option réinstalle le/les fichier(s) /le module qui éxecute le script";
			this.header21.Text = "Installez les dépendances";
			this.header_Copy22.Text = "Réinstalle VC Redist x64, x86 et DirectX automatiquement";
			this.no_results = "Désolé, nous n'avons pas pu trouver de résultats. :(";
			this.save_text = "Vos paramètres ont été sauvegardés et se chargeront automatiquement la prochaine fois que vous lancerez Delta !";
			this.auto_exec_prompt = "Cela ne peut pas être activé sur Activé/Désactivé parce que Delta éxecute automatiquement tous les fichiers que vous mettez dans le dossier \"autoexec\".\nVoulez-vous ouvrir le dossier \"autoexec\"?";
			this.exec_btn.Content = "ÉXECUTER";
			this.clr_btn.Content = "RETIRER";
			this.opn_btn.Content = "OUVRIR";
			this.sve_btn.Content = "SAUVEGARDER";
			this.inj_btn.Content = "INJECTER";
			this.execute_again_button.Content = "Éxecuter";
			this.browse_lbl.Text = "Parcourir";
			this.hot.Content = "Excellent";
			this.old.Content = "Plus vieux";
			this.mostv.Content = "Plus vus";
			this.FAVS.Content = "Préférés";
			this.desctxt.Text = "Description";
			this.search_scripts_folder_box.Tag = "Filtrer les éléments..";
			this.searchbox.Tag = "Filtrer les éléments..";
			this.allselected.Content = "LES DEUX";
			this.placeholdertext.Text = "Nous avons environ plus de 4000 scripts que vous pouvez rechercher ! Commencez en recherchant :D";
			this.theme_title.Text = "Sélectionnez un thème !";
			this.theme_desc.Text = "Sélectionnez un thème pour commencer ! Vous pouvez l'appliquer en utilisant le bouton \"Utiliser\"";
			this.resettheme.Content = "Réinitialiser";
			this.usetheme.Content = "Utiliser";
			this.homelbl.Text = "ACCUEIL";
			this.scrptslbl.Text = "SCRIPTS";
			this.thmlbl.Text = "THÈMES";
			this.cnfglbl.Text = "CONFIGS";
			this.header_Copy4.Text = "Configuration";
			this.saveset.Content = "Sauvegarder";
		}

		public void thailang()
		{
			this.header1.Text = "รันสคริปอัตโนมัติ";
			this.header_Copy1.Text = "สิ่งนี้จะรันสคริปต์ทั้งหมดที่คุณต้องการโดยอัตโนมัติทุกครั้งที่คุณฉีด";
			this.header9.Text = "เปิดไฟล์ตัวฉีด";
			this.header_Copy10.Text = "เปิดโฟลเดอร์ DLL ของ Delta ซึ่งมีเวิร์กสเปซ บันทึก และโฟลเดอร์ autoexec";
			this.header2.Text = "ปลดล็อค FPS";
			this.header_Copy2.Text = "ปลดล็อกเฟรมต่อวินาทีเพื่อรับประสบการณ์ที่ดียิ่งขึ้นใน Roblox";
			this.header3.Text = "ปิด Roblox";
			this.header_Copy3.Text = "สิ่งนี้จะปิดกระบวนการ Roblox ที่กำลังทำงานอยู่ทั้งหมด";
			this.header4.Text = "โปร่งใส";
			this.header_Copy5.Text = "ทำให้เดลต้าโปร่งใสโดยลดความทึบของหน้าต่าง";
			this.header5.Text = "โฟกัส Delta";
			this.header_Copy6.Text = "ทำให้ Delta โฟกัสอยู่ที่ด้านบนของหน้าต่างอื่นๆ ของคุณ";
			this.header6.Text = "ล่องหนอัตโนมัติ";
			this.header_Copy7.Text = "ลดความทึบโดยอัตโนมัติเมื่อโฟกัสของหน้าต่างหายไป ( ควรใช้กับ โฟกัส Delta )";
			this.header12.Text = "ฉีดแบบ Delta";
			this.header_Copy13.Text = "ใช้ตัวฉีดแบบ Delta ระดับ 7 อันทรงพลัง (แนะนำ)";
			this.header13.Text = "ฉีดแบบ WRD";
			this.header_Copy14.Text = "ใช้ตัวฉีดแบบ WRD (ระดับ 7)";
			this.header8.Text = "ฉีดอัตโนมัติ";
			this.header_Copy9.Text = "สิ่งนี้จะฉีดให้คุณโดยอัตโนมัติทุกครั้งที่คุณเปิด Roblox";
			this.header7.Text = "ตรวจสอบรุ่น";
			this.header_Copy8.Text = "แก้ไขเวอร์ชั่น roblox หากผิด (อาจทำให้การฉีดช้า)";
			this.header20.Text = "ดาวน์โหลดฉีดแบบ Delta ใหม่";
			this.header_Copy21.Text = "ตัวเลือกนี้จะติดตั้ง DLL ของ Delta ใหม่ซึ่งเรียกใช้สคริปต์";
			this.header21.Text = "ดาวน์โหลดตัวพึ่งพา";
			this.header_Copy22.Text = "ติดตั้ง VC Redist x64, x86 และ DirectX ใหม่โดยอัตโนมัติ";
			this.no_results = "พวกเราไม่สามารถหาสคริปได้ :(";
			this.save_text = "การตั้งค่าของคุณได้รับการบันทึกแล้ว และจะโหลดโดยอัตโนมัติในครั้งถัดไปที่คุณเปิดใช้ Delta!";
			this.auto_exec_prompt = "ไม่สามารถเปิด/ปิดได้เนื่องจาก Delta เรียกใช้ไฟล์ทั้งหมดที่คุณใส่ในโฟลเดอร์ \"autoexec\" โดยอัตโนมัติ\nคุณต้องการเปิดโฟลเดอร์ \"autoexec\" หรือไม่";
			this.exec_btn.Content = "รันสคริป";
			this.clr_btn.Content = "ลบ";
			this.opn_btn.Content = "เปิดสคริป";
			this.sve_btn.Content = "บันทึกสคริป";
			this.inj_btn.Content = "ฉีด";
			this.execute_again_button.Content = "รันสคริป";
			this.browse_lbl.Text = "ค้นหา";
			this.hot.Content = "มาแรง";
			this.old.Content = "เก่าที่สุด";
			this.mostv.Content = "ดูมากที่สุด";
			this.FAVS.Content = "ชอบ";
			this.desctxt.Text = "คำอธิบาย";
			this.search_scripts_folder_box.Tag = "หาสคริป";
			this.searchbox.Tag = "หาสคริป";
			this.allselected.Content = "ทุกไฟล์";
			this.placeholdertext.Text = "เรามีสคริปมากกว่า 4,000 รายการที่คุณสามารถค้นหาได้! เริ่มต้นด้วยการค้นหา :D";
			this.theme_title.Text = "เลือกธีม!";
			this.theme_desc.Text = "เลือกธีมในแปปของคุณ! โดยใช้ปุ่ม ใช้ธีม";
			this.resettheme.Content = "ลบธีม";
			this.usetheme.Content = "ใช้ธีม";
			this.homelbl.Text = "ตัวรัน";
			this.scrptslbl.Text = "หาสคริป";
			this.thmlbl.Text = "ธีม";
			this.cnfglbl.Text = "ตั้งค่า";
			this.header_Copy4.Text = "กำหนดค่า";
			this.saveset.Content = "บันทึกการตั้งค่า";
		}

		public void polishlang()
		{
			this.header1.Text = "Automatycznie Uzywanie Skryptów";
			this.header_Copy1.Text = "To automatycznie uzywa wszystkie skrypty ktore chcesz uzywać gdy wstrzykujesz lub zmieniasz serwery";
			this.header9.Text = "Włacz Folder DLL";
			this.header_Copy10.Text = "To Włacza Folder Delta DLL, to obejmuje obszar roboczy, logi i folder autoexec ";
			this.header2.Text = "Odblokuj Klatki Na sekundę";
			this.header_Copy2.Text = "Odblokuj swoje klatki na sekundę, aby uzyskać znacznie lepsze wrażenia z Roblox";
			this.header3.Text = "Zabij Roblox";
			this.header_Copy3.Text = "Zabija/Wylacza wszystkie processy robloxa";
			this.header4.Text = "Przezroczystość";
			this.header_Copy5.Text = "Sprawia że ​​Delta staje się przezroczysta zmniejszając przezroczystość okna";
			this.header5.Text = "Nad Oknami";
			this.header_Copy6.Text = "Sprawia, że ​​interfejs Delta pozostaje skupiony na innych oknach";
			this.header6.Text = "Automatyczne zanikanie";
			this.header_Copy7.Text = "Automatycznie zmniejszaj krycie po utracie ostrości okna";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "Użyj potężnego interfejsu Delta poziomu 7 (zalecane)";
			this.header13.Text = "WRD API";
			this.header_Copy14.Text = "Użyj API WeAreDevs (poziom 7)";
			this.header8.Text = "Automatyczny Wstrzyk";
			this.header_Copy9.Text = "To automatycznie wstrzykuje ci za każdym razem, gdy otwierasz roblox";
			this.header7.Text = "Sprawdzanie wersji";
			this.header_Copy8.Text = "Poprawia wersję roblox, jeśli jest zła (może spowolnić wtrysk)";
			this.header20.Text = "Ponownie zainstaluj bibliotekę DLL";
			this.header_Copy21.Text = "Ta opcja powoduje ponowną instalację biblioteki DLL lub modułu Delta, który uruchamia skrypty";
			this.header21.Text = "Zainstaluj zależności";
			this.header_Copy22.Text = "Ponownie zainstaluj automatycznie VC Redist x64, x86 i DirectX";
			this.no_results = "Przepraszamy, nie znaleźliśmy żadnych wyników :(";
			this.save_text = "Twoje ustawienia zostały zapisane i zostaną automatycznie załadowane przy następnym uruchomieniu Delta!";
			this.auto_exec_prompt = "TNie można tego włączyć/wyłączyć, ponieważ Delta automatycznie uruchamia wszystkie pliki, które umieściłeś w folderze \"autoexec\".\nCzy chcesz otworzyć folder \"autoexec\"?";
			this.exec_btn.Content = "WYKONAĆ";
			this.clr_btn.Content = "WYCZYŚĆ";
			this.opn_btn.Content = "WŁĄCZ";
			this.sve_btn.Content = "ZAPISZ";
			this.inj_btn.Content = "WSTRZYKNIJ";
			this.execute_again_button.Content = "Wykonać";
			this.browse_lbl.Text = "Przeglądaj";
			this.hot.Content = "Gorące";
			this.old.Content = "Najstarszy";
			this.mostv.Content = "Najczęściej oglądane";
			this.FAVS.Content = "Ulubione";
			this.desctxt.Text = "Opis";
			this.search_scripts_folder_box.Tag = "Filtruj elementy..";
			this.searchbox.Tag = "Filtruj elementy..";
			this.allselected.Content = "WSZYSTKIE PLIKI";
			this.placeholdertext.Text = "Mamy ponad 4000 skryptów, które możesz wyszukać! Zacznij od szukania :D";
			this.theme_title.Text = "Wybierz motyw!";
			this.theme_desc.Text = "Wybierz motyw, aby rozpocząć! Zastosuj go za pomocą przycisku \"Użyj motywu\"";
			this.resettheme.Content = "Zresetuj motyw";
			this.usetheme.Content = "Uzyj Motyw";
			this.homelbl.Text = "DOM";
			this.scrptslbl.Text = "SKRYPTY";
			this.thmlbl.Text = "MOTYWY";
			this.cnfglbl.Text = "KONFIG";
			this.header_Copy4.Text = "Konfiguracja";
			this.saveset.Content = "Zapisz Ustawienia";
		}

		public void filipinolang()
		{
			this.header1.Text = "Auto Execute";
			this.header_Copy1.Text = "Awtomatiko nitong pinapagana ang lahat ng mga script na gusto mo sa tuwing mag-inject ka/serverhop";
			this.header9.Text = "Open ang DLL Folder";
			this.header_Copy10.Text = "Magopen Delta's DLL Folder, kasama ang workspace, logs at autoexec folder";
			this.header2.Text = "FPS Unlocker";
			this.header_Copy2.Text = "I-unlock ang iyong mga frame sa bawat segundo para makakuha ng mas pinahusay na karanasan sa Roblox";
			this.header3.Text = "Kill Roblox";
			this.header_Copy3.Text = "ito ay nagsasarado lahat nangyayari na Roblox processes";
			this.header4.Text = "Opacity";
			this.header_Copy5.Text = "Ginagawang transparent ang Delta sa pamamagitan ng pagpapababa sa opacity ng window";
			this.header5.Text = "TopMost";
			this.header_Copy6.Text = "Ginagawang manatiling nakatutok ang Delta UI sa itaas ng iyong iba pang mga windows";
			this.header6.Text = "Auto Fade";
			this.header_Copy7.Text = "Awtomatikong babaan ang opacity kapag nawala ang focus sa window";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "gamitin ang malakas na level 7 Delta API (recomended)";
			this.header13.Text = "WRD API";
			this.header_Copy14.Text = "Gamitin ang WeAreDevs API (level 7)";
			this.header8.Text = "Auto Inject";
			this.header_Copy9.Text = "Awtomatikong nag-iinject ito para sa iyo sa tuwing bubuksan mo ang roblox";
			this.header7.Text = "pagsusuri ng bersyon";
			this.header_Copy8.Text = "Itinatama ang bersyon ng roblox kung mali ito (pwede bumagal ang injection)";
			this.header20.Text = "muling i-install ang dll";
			this.header_Copy21.Text = "Ang pagpipiliang ito ay muling nag-install ng Delta DLL/Module ng Delta na nagpapatakbo ng mga script";
			this.header21.Text = "I-install ang Dependencies";
			this.header_Copy22.Text = "I-install muli VC Redist x64, x86 at DirectX automatically";
			this.no_results = "Paumanhin, wala kaming mahanap na anumang resulta :(";
			this.save_text = "ang mga settings mo ay na save at awtomatikong maglo-load sa susunod na ilunsad mo ang Delta!";
			this.auto_exec_prompt = "Hindi ito kaya i on/off dahil Delta nag awtomatikong ipapatupad lahat ng files na nilagay sa\"autoexec\" folder.\nGusto mo ba i buksan ang \"autoexec\" folder?";
			this.exec_btn.Content = "BUMITAY";
			this.clr_btn.Content = "TANGGALIN";
			this.opn_btn.Content = "BUKSAN";
			this.sve_btn.Content = "ILIGTAS";
			this.inj_btn.Content = "INJECTO";
			this.execute_again_button.Content = "Bumitay";
			this.browse_lbl.Text = "Paghahanap";
			this.hot.Content = "Sikat";
			this.old.Content = "Matanda";
			this.mostv.Content = "Pinaka viewed";
			this.FAVS.Content = "Paborito";
			this.desctxt.Text = "Paglalarawan";
			this.search_scripts_folder_box.Tag = "ifilter ang mga gamit..";
			this.searchbox.Tag = "ifilter ang mga gamit..";
			this.allselected.Content = "Lahat ng FILES";
			this.placeholdertext.Text = "Meron kaming lagpas na 4000+ skripts na kaya mo i gamitin ! magsimula sa pamamagitan ng paghahanap :D";
			this.theme_title.Text = "Pumili ng Tema!";
			this.theme_desc.Text = "Mag select ng theme para mag start! gamitin mo ito gamit ang \"Gamitin\" pindutan";
			this.resettheme.Content = "Reset Tema";
			this.usetheme.Content = "Gamitin";
			this.homelbl.Text = "HOME";
			this.scrptslbl.Text = "SKRIPTS";
			this.thmlbl.Text = "TEMAS";
			this.cnfglbl.Text = "CONFIG";
			this.header_Copy4.Text = "pagsasaayos";
			this.saveset.Content = "i-save ang mga setting";
		}

		public void germanlang()
		{
			this.header1.Text = "Auto Execute";
			this.header_Copy1.Text = "Diese Einstellung führt alle skripte beim injecten oder serverhoppen aus";
			this.header9.Text = "DLL Ordner öffnen";
			this.header_Copy10.Text = "Öffnet Delta's DLL Order, wo der workspace, logs und autoexec ordner ist";
			this.header2.Text = "FPS Entsperrer";
			this.header_Copy2.Text = "Entsperrt deine FPS, um Roblox viel besser zu erleben";
			this.header3.Text = "Schliesse Roblox";
			this.header_Copy3.Text = "Beendet/Schließt alle derzeit laufenden Roblox-Prozesse";
			this.header4.Text = "Transparenz";
			this.header_Copy5.Text = "Macht Delta durchsichtig bzw. Transparent";
			this.header5.Text = "TopMost";
			this.header_Copy6.Text = "Lässt die Delta-Benutzeroberfläche über Ihren anderen Fenstern fokussiert bleiben";
			this.header6.Text = "Auto Fade";
			this.header_Copy7.Text = "Verringert die Deckkraft automatisch, wenn ein anderes Fenster fokussiert wird";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "Verwende die leistungsfähige Level-7-Delta-API (empfohlen)";
			this.header13.Text = "WRD API";
			this.header_Copy14.Text = "Verwende die WeAreDevs API (level 7)";
			this.header8.Text = "Auto Inject";
			this.header_Copy9.Text = "Injected Delta automatisch, wenn roblox geöffnet wird";
			this.header7.Text = "Versions Kontrolle";
			this.header_Copy8.Text = "Korrigiert die Roblox-Version, wenn sie falsch ist (könnte die Injection verlangsamen)";
			this.header20.Text = "Reinstall DLL";
			this.header_Copy21.Text = "Diese Option installiert Deltas DLL/Modul, das Skripte ausführt, nochmal";
			this.header21.Text = "Install Dependencies";
			this.header_Copy22.Text = "Installieren Sie VC Redist x64, x86 und DirectX automatisch neu";
			this.no_results = "Sorry, wir konnten keine Ergebnisse finden :(";
			this.save_text = "Die Einstellungen wurden gespeichert und werden automatisch geladen, wenn Delta das nächste Mal geöffnet wird!";
			this.auto_exec_prompt = "Das kann nicht ein-/ausgeschaltet werden, weil Delta automatisch alle Dateien ausführt, die im Ordner \"autoexec\" abgelegt sind.\nSoll der Ordner \"autoexec\" geöffnet werden?";
			this.exec_btn.Content = "AUSFÜHREN";
			this.clr_btn.Content = "LÖSCHEN";
			this.opn_btn.Content = "ÖFFNEN";
			this.sve_btn.Content = "SPEICHERN";
			this.inj_btn.Content = "INJECT";
			this.execute_again_button.Content = "Ausführen";
			this.browse_lbl.Text = "Suchen";
			this.hot.Content = "Im Trend";
			this.old.Content = "Älteste";
			this.mostv.Content = "Meiste Aufrufe";
			this.FAVS.Content = "Favoriten";
			this.desctxt.Text = "Beschreibung";
			this.search_scripts_folder_box.Tag = "Im Ordner suchen..";
			this.searchbox.Tag = "Skripte suchen..";
			this.allselected.Content = "ALLE [ * ]";
			this.placeholdertext.Text = "Delta hat über 4.000 Skripte nach denen du suchen kannst! Probier's doch aus :D";
			this.theme_title.Text = "Wähle ein Design aus!";
			this.theme_desc.Text = "Wähle ein Design aus! Du kannst es mit dem \"Verwenden\" Knopf benutzen!";
			this.resettheme.Content = "Standard";
			this.usetheme.Content = "Verwenden";
			this.homelbl.Text = "START";
			this.scrptslbl.Text = "SKRIPTE";
			this.thmlbl.Text = "DESIGNS";
			this.cnfglbl.Text = "KONFIG";
			this.header_Copy4.Text = "Einstellungen";
			this.saveset.Content = "Speichern";
		}

		public void turkishlang()
		{
			this.header1.Text = "Otomatik Yürüt";
			this.header_Copy1.Text = "Bu, her enjekte ettiğinizde/serverhop yaptığınızda istediğiniz tüm komut dosyalarını otomatik olarak yürütür";
			this.header9.Text = "DLL Klasörünü Aç";
			this.header_Copy10.Text = "Bu, çalışma alanını, günlükleri ve autoexec klasörünü içeren Delta'nın DLL Klasörünü açar";
			this.header2.Text = "FPS Kilidi Açıcı";
			this.header_Copy2.Text = "Roblox'ta çok daha gelişmiş bir deneyim elde etmek için saniyedeki karelerinizin kilidini açın";
			this.header3.Text = "Roblox'u Öldür";
			this.header_Copy3.Text = "Bu, şu anda çalışan tüm Roblox işlemlerini öldürür/kapatır";
			this.header4.Text = "Opaklık";
			this.header_Copy5.Text = "Pencere opaklığını düşürerek Delta'yı şeffaf yapar";
			this.header5.Text = "TopMost";
			this.header_Copy6.Text = "Delta Kullanıcı Arayüzünün diğer pencerelerinizin üzerinde odaklanmış durumda kalmasını sağlar";
			this.header6.Text = "Otomatik Solma";
			this.header_Copy7.Text = "Pencere odağı kaybolduğunda opaklığı otomatik olarak azalt";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "Güçlü seviye 7 Delta API'sini kullanın (önerilir)";
			this.header13.Text = "WRD API";
			this.header_Copy14.Text = "WeAreDevs API'sini kullanın (seviye 7)";
			this.header8.Text = "Otomatik Enjeksiyon";
			this.header_Copy9.Text = "Bu, roblox'u her açtığınızda sizin için otomatik olarak enjekte eder";
			this.header7.Text = "Sürüm Kontrolü";
			this.header_Copy8.Text = "Yanlış ise roblox sürümünü düzeltir (Enjeksiyon yavaşlayabilir)";
			this.header20.Text = "DLL'yi Yeniden Yükle";
			this.header_Copy21.Text = "Bu seçenek, Delta'nın komut dosyalarını çalıştıran DLL/Modülünü yeniden yükler";
			this.header21.Text = "Bağımlılıkları Kur";
			this.header_Copy22.Text = "VC Redist x64, x86 ve DirectX'i otomatik olarak yeniden kurun";
			this.no_results = "Üzgünüz, herhangi bir sonuç bulamadık :(";
			this.save_text = "Ayarlarınız kaydedildi ve Delta'yı bir sonraki başlatışınızda otomatik olarak yüklenecek!";
			this.auto_exec_prompt = "Delta, \"autoexec\" klasörüne koyduğunuz tüm dosyaları otomatik olarak yürüttüğü için bu açılamaz/kapatılamaz.\n\"autoexec\" klasörünü açmak istiyor musunuz";
			this.exec_btn.Content = "YÜRÜT";
			this.clr_btn.Content = "SİL";
			this.opn_btn.Content = "AÇIK";
			this.sve_btn.Content = "KAYDET";
			this.inj_btn.Content = "ENJEKT";
			this.execute_again_button.Content = "Yürüt";
			this.browse_lbl.Text = "Gözt";
			this.hot.Content = "Sıcak";
			this.old.Content = "En Eski";
			this.mostv.Content = "En Çok Görüntülenen";
			this.FAVS.Content = "Favoriler";
			this.desctxt.Text = "Açıklama";
			this.search_scripts_folder_box.Tag = "Öğeleri Filtrele..";
			this.searchbox.Tag = "Öğeleri Filtrele..";
			this.allselected.Content = "TÜM DOSYALAR";
			this.placeholdertext.Text = "Arayabileceğiniz 4.000'den fazla Komut Dosyamız var! Bir arama yaparak başlayın :D";
			this.theme_title.Text = "Bir Tema Seçin!";
			this.theme_desc.Text = "Başlamak için bir Tema seçin! \"Temayı Kullan\" Düğmesini kullanarak uygulayın";
			this.resettheme.Content = "Temayı Sıfırla";
			this.usetheme.Content = "Temayı Kullan";
			this.homelbl.Text = "GİRİŞ";
			this.scrptslbl.Text = "SCRIPTS";
			this.thmlbl.Text = "TEMALAR";
			this.cnfglbl.Text = "KONFİG";
			this.header_Copy4.Text = "Yapılandırma";
			this.saveset.Content = "Ayarları Kaydet";
		}

		public void spanishlang()
		{
			this.header1.Text = "Ejecutado Automatico";
			this.header_Copy1.Text = "Automaticamente ejecuta todos los scripts que quieres cada vez que inyectas/cambias de servidor";
			this.header9.Text = "Abrir La Carpeta De DLL";
			this.header_Copy10.Text = "Abre la carpeta de DLL de Delta, incluye las carpetas de workspace, logs y autoejecutado";
			this.header2.Text = "FPS Unlocker";
			this.header_Copy2.Text = "Desbloquea los cuadros por segundo para una experiencia mejorada en Roblox";
			this.header3.Text = "Cerrar Roblox";
			this.header_Copy3.Text = "Cierra todos los procesos relacionados con Roblox";
			this.header4.Text = "Opacidad";
			this.header_Copy5.Text = "Hace que Delta sea transparente disminuyendo la opacidad de la ventana.+";
			this.header5.Text = "TopMost";
			this.header_Copy6.Text = "Hace que la IU de Delta se mantenga encima de todas las ventanas";
			this.header6.Text = "Auto Fade";
			this.header_Copy7.Text = "Automaticamente disminuye la opacidad de la ventana si no la estas usando";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "Usa la poderosa API Delta nivel 7 (recomendado)";
			this.header13.Text = "WRD API";
			this.header_Copy14.Text = "Usa la API WeAreDevs (nivel 7)";
			this.header8.Text = "Inyectado Automatico";
			this.header_Copy9.Text = "Automaticamente inyecta Delta cuando abres Roblox";
			this.header7.Text = "Verificacion De Version";
			this.header_Copy8.Text = "Corrige la version de Roblox si es incorrecta (Puede causar lentitud al inyectar)";
			this.header20.Text = "Reinstalar DLL";
			this.header_Copy21.Text = "Esta opcion reinstala el modulo/dll de Delta el cual ejecuta scripts.";
			this.header21.Text = "Instalar Dependencias";
			this.header_Copy22.Text = "Reinstalar VC Redist x64, x86 y DirectX automaticamente";
			this.no_results = "Lo sentimos, no podimos encontrar ningun resultado :C";
			this.save_text = "!Tu configuracion fue guardada y se aplicara cuando abras Delta de nuevo¡";
			this.auto_exec_prompt = "Esto no puede ser habilitado/desabilitado porque Delta automaticamente ejecuta todos los archivos que colocas en la carpeta \"autoexec\" .\n¿Quieres abrir el \"autoexec\" folder?";
			this.exec_btn.Content = "EJECUTAR";
			this.clr_btn.Content = "BORRAR";
			this.opn_btn.Content = "ABRIR";
			this.sve_btn.Content = "GUARDAR";
			this.inj_btn.Content = "INYECTAR";
			this.execute_again_button.Content = "Ejecutar";
			this.browse_lbl.Text = "Navegar";
			this.hot.Content = "Tendencia";
			this.old.Content = "Antiguo";
			this.mostv.Content = "Mas Visitado";
			this.FAVS.Content = "Favoritos";
			this.desctxt.Text = "Descripcion";
			this.search_scripts_folder_box.Tag = "Filtrar elementos..";
			this.searchbox.Tag = "Filtrar elementos..";
			this.allselected.Content = "TODOS LOS ARCHIVOS";
			this.placeholdertext.Text = "¡Tenemos mas de 4000+ scripts que puedes buscar! Intenta hacer una busqueda :D";
			this.theme_title.Text = "¡Elige un tema!";
			this.theme_desc.Text = "¡Elige un tema para empezar! Aplicalo usando el boton \"Usar Tema\"";
			this.resettheme.Content = "Tema Predeterminado";
			this.usetheme.Content = "Usar Tema";
			this.homelbl.Text = "INICIO";
			this.scrptslbl.Text = "SCRIPTS";
			this.thmlbl.Text = "TEMAS";
			this.cnfglbl.Text = "AJUSTES";
			this.header_Copy4.Text = "Ajustes";
			this.saveset.Content = "Guardar Ajustes";
		}

		public void indonesianlang()
		{
			this.header1.Text = "Otomatis Jalankan";
			this.header_Copy1.Text = "Ini akan otomatis mengjalankan semua script yang kamu mau setiap kamu menginjeksi atau melakukan server hop(berpindah server)";
			this.header9.Text = "Buka Folder DLL";
			this.header_Copy10.Text = "Ini Akan Membuka folder DLL Delta , yang meliputi workspace, logs(histori) dan Folder Otomatis Jalankan ";
			this.header2.Text = "Membuka FPS";
			this.header_Copy2.Text = "Ini Akan membuka fps kamu agar kamu mendapatkan pengalaman yang lebih baik dan berkualitas di roblox";
			this.header3.Text = "Tutup Roblox";
			this.header_Copy3.Text = "Ini Akan menutup semua proses roblox yang sedang berjalan";
			this.header4.Text = "Opasitas";
			this.header_Copy5.Text = "Ini Akan membuat delta transparan dengan menurunkan opasitas window kamu";
			this.header5.Text = "Paling Atas";
			this.header_Copy6.Text = "Ini Akan membuat UI Delta tetap berada di paling depan/atas diantara windows yang lain";
			this.header6.Text = "Pudar Otomatis";
			this.header_Copy7.Text = "Ini Akan Membuat Delta otomatis menurunkan opasitas saat pengguna melihat windows lain";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "Gunakan Level 7 Delta APi Yang Kuat(Rekomendasi)";
			this.header13.Text = "WRD API";
			this.header_Copy14.Text = "Gunakan WeAreDevs APi (Level 7)";
			this.header8.Text = "Otomatis Injeksi";
			this.header_Copy9.Text = "Ini Akan Membuat Delta Otomatis Menginjeksi untuk kamu setiap kamu membuka roblox";
			this.header7.Text = "Pemeriksa Versi";
			this.header_Copy8.Text = "Memperbaiki versi roblox jika versi tersebut salah (bisa memperlambat injeksi)";
			this.header20.Text = "Instal Ulang DLL";
			this.header_Copy21.Text = "Opsi Ini akan menginstal ulang Dll/Modul yang fungsinya menjalankan script";
			this.header21.Text = "Instal Dependencies";
			this.header_Copy22.Text = "Menginstal Ulang VC Redist x64, x86 dan DirectX Secara Otomatis";
			this.no_results = "Maaf, Kita Tidak Bisa Menemukan Apapun :(";
			this.save_text = "Pengaturan Anda Telah Disimpan Telah Disimpan Dan Akan Dimuat Secara Otomatis Saat Anda Membuka Delta Lagi!";
			this.auto_exec_prompt = "Ini Tidak Bisa Dinyalakan/Dimatikan karena Delta Secara Otomatis Menjalankan Semua File/Script Yang Anda masukan ke dalam folder \"otomatisjalankan\" .\nApakah Kamu Mau Membuka \"otomatisjalankan\" folder?";
			this.exec_btn.Content = "JALANKAN";
			this.clr_btn.Content = "BERSIHKAN";
			this.opn_btn.Content = "BUKA";
			this.sve_btn.Content = "SIMPAN";
			this.inj_btn.Content = "INJEKSI";
			this.execute_again_button.Content = "Jalankan";
			this.browse_lbl.Text = "Cari";
			this.hot.Content = "Populer";
			this.old.Content = "Paling Tua";
			this.mostv.Content = "Paling Banyak Dilihat";
			this.FAVS.Content = "Favorit";
			this.desctxt.Text = "Deskripsi";
			this.search_scripts_folder_box.Tag = "Sortir Item..";
			this.searchbox.Tag = "Sortir Items..";
			this.allselected.Content = "SEMUA FILE";
			this.placeholdertext.Text = "Kami Memiliki Lebih Dari 4.000+ Skrip Yang Anda Dapat Cari! Mulailah Dengan Melakukan Pencarian :D";
			this.theme_title.Text = "Select a Theme!";
			this.theme_desc.Text = "Pilih Tema Untuk Mulai! Terapkan Dengan \"Gunakan\" Tombol";
			this.resettheme.Content = "Ulang Tema";
			this.usetheme.Content = "Gunakan";
			this.homelbl.Text = "RUMAH";
			this.scrptslbl.Text = "SKRIP";
			this.thmlbl.Text = "TEMA";
			this.cnfglbl.Text = "KONFIG";
			this.header_Copy4.Text = "Konfigurasi";
			this.saveset.Content = "Simpan";
		}

		public void portlang()
		{
			this.header1.Text = "Auto execucao";
			this.header_Copy1.Text = "Isso vai executar automaticamente todos os scripts que voce quer toda vez que voce injetar/trocar de server";
			this.header9.Text = "Abrir DLL Folder";
			this.header_Copy10.Text = "Isso abre o Delta DLL Folder, isso inclui a area de trabalho, historico e a pasta de auto execucao ";
			this.header2.Text = "Desbloqueador de FSP";
			this.header_Copy2.Text = "Desbloqueia os frames por segundo para obter uma experiencia muito mais aprimorada no Roblox";
			this.header3.Text = "Kill Roblox";
			this.header_Copy3.Text = "Isso mata/fecha todos os roblox em processo";
			this.header4.Text = "Opacidade";
			this.header_Copy5.Text = "Torna o delta transparente ao baixar a opacidade da janela";
			this.header5.Text = "Acima";
			this.header_Copy6.Text = "Faz a UI do Delta continuar acima das outras janelas";
			this.header6.Text = "Auto Fade";
			this.header_Copy7.Text = "diminui a opacidade automaticamente quando a janela esta sem foco";
			this.header12.Text = "Delta API";
			this.header_Copy13.Text = "Usa o poderoso level 7 Delta API (recommended)";
			this.header13.Text = "WRD API";
			this.header_Copy14.Text = "Usa o WeAreDevs API (level 7)";
			this.header8.Text = "Injetar automaticamente";
			this.header_Copy9.Text = "Isso injeta automaticamente para voce toda vez que voce abrir o roblox";
			this.header7.Text = "Checando versao";
			this.header_Copy8.Text = "Corrige a vers?o do roblox se estiver errada (PODE desacelerar a injetar)";
			this.header20.Text = "Reinstala DLL";
			this.header_Copy21.Text = "Essa opcao reinstala a DLL/Modulo do Delta que executar os scripts";
			this.header21.Text = "Instala dependencias";
			this.header_Copy22.Text = "Reinstala VC Redist x64, x86 e o DirectX automaticamente";
			this.no_results = "Desculpa, nos nao encontramos nenhum resultado :(";
			this.save_text = "As configuracoes sao salvas e sao automaticamente carregadas quando voce abre o Delta!";
			this.auto_exec_prompt = "Isso nao pode ser ligado/desligado porque o Delta automaticamente executa todos os arquivos que voce coloca em \"autoexec\" folder.\nDo voce quer abrir a pasta \"autoexec\" ?";
			this.exec_btn.Content = "EXECUTAR";
			this.clr_btn.Content = "LIMPAR";
			this.opn_btn.Content = "ABRIR";
			this.sve_btn.Content = "SALVAR";
			this.inj_btn.Content = "INJETAR";
			this.execute_again_button.Content = "Executar";
			this.browse_lbl.Text = "Navegar";
			this.hot.Content = "Tendendo";
			this.old.Content = "Antigo";
			this.mostv.Content = "Mais visto";
			this.FAVS.Content = "Favoritos";
			this.desctxt.Text = "Descricao";
			this.search_scripts_folder_box.Tag = "Filtrar Itens..";
			this.searchbox.Tag = "Filtrar Itens..";
			this.allselected.Content = "TODOS OS ARQUIVOS";
			this.placeholdertext.Text = "Nos temos mais de 4,000+ Scripts that voce pode pesquisar! Comece fazendo uma pesquisa :D";
			this.theme_title.Text = "Selecione um Tema!";
			this.theme_desc.Text = "Selecione um tema para comecar! Aplique-o clicando no botao \"Use Theme\" ";
			this.resettheme.Content = "Resetar Tema";
			this.usetheme.Content = "Usar Tema";
			this.homelbl.Text = "INICIO";
			this.scrptslbl.Text = "SCRIPTS";
			this.thmlbl.Text = "TEMAS";
			this.cnfglbl.Text = "CONFIG";
			this.header_Copy4.Text = "Configuracao";
			this.saveset.Content = "Salvar config";
		}

		void dispatcherTimer1_Tick(object sender, EventArgs e)
		{
			if (DLLPipe.NamedPipeExist())
			{
				this.DT1.Stop();
				this.DT.Start();
			}
		}

		void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			XWindow.<dispatcherTimer_Tick>d__63 <dispatcherTimer_Tick>d__;
			<dispatcherTimer_Tick>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<dispatcherTimer_Tick>d__.<>4__this = this;
			<dispatcherTimer_Tick>d__.<>1__state = -1;
			<dispatcherTimer_Tick>d__.<>t__builder.Start<XWindow.<dispatcherTimer_Tick>d__63>(ref <dispatcherTimer_Tick>d__);
		}

		void Button_Click(object sender, RoutedEventArgs e)
		{
			this.create_a_new_tab("NewScript.lua", "");
		}

		void button_Click_1(object sender, RoutedEventArgs e)
		{
			if (this.tabControl1.Items.Count != 1)
			{
				this.tabControl1.Items.Remove(this.tabControl1.SelectedItem);
			}
		}

		void allselected_Checked(object sender, RoutedEventArgs e)
		{
			this.luaselected.IsChecked = new bool?(false);
			this.txtselected.IsChecked = new bool?(false);
			this.refresh_scriptbox("");
		}

		void luaselected_Checked(object sender, RoutedEventArgs e)
		{
			this.allselected.IsChecked = new bool?(false);
			this.txtselected.IsChecked = new bool?(false);
			this.refresh_scriptbox("");
		}

		void txtselected_Checked(object sender, RoutedEventArgs e)
		{
			this.luaselected.IsChecked = new bool?(false);
			this.allselected.IsChecked = new bool?(false);
			this.refresh_scriptbox("");
		}

		void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				base.DragMove();
			}
			catch
			{
			}
		}

		void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.refresh_scriptbox(this.search_scripts_folder_box.Text);
		}

		void Button_Click_2(object sender, RoutedEventArgs e)
		{
			this.SetText(string.Empty);
		}

		void Button_Click_3(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Delta | " + this.opn_btn.Content.ToString().Substring(0, 1) + this.opn_btn.Content.ToString().Substring(1).ToLower();
			openFileDialog.Filter = "Text Files|*.txt|Lua Files|*.lua|All Files|*.*";
			if (openFileDialog.ShowDialog().Value)
			{
				this.create_a_new_tab(openFileDialog.SafeFileName, "");
				this.SetText(File.ReadAllText(openFileDialog.FileName));
			}
		}

		void Button_Click_4(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = ((this.GetCurrentTab().Header as TextBox).Text ?? "");
			saveFileDialog.Title = "Delta | " + this.sve_btn.Content.ToString().Substring(0, 1) + this.sve_btn.Content.ToString().Substring(1).ToLower();
			saveFileDialog.Filter = "Text Files|*.txt|Lua Files|*.lua|All Files|*.*";
			if (saveFileDialog.ShowDialog().Value)
			{
				File.WriteAllText(saveFileDialog.FileName, this.GetText());
			}
		}

		void autoexec_Checked(object sender, RoutedEventArgs e)
		{
			XWindow.<autoexec_Checked>d__74 <autoexec_Checked>d__;
			<autoexec_Checked>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<autoexec_Checked>d__.<>4__this = this;
			<autoexec_Checked>d__.<>1__state = -1;
			<autoexec_Checked>d__.<>t__builder.Start<XWindow.<autoexec_Checked>d__74>(ref <autoexec_Checked>d__);
		}

		void Grid_ContextMenuClosing(object sender, ContextMenuEventArgs e)
		{
		}

		void autoupdate_Copy_Checked(object sender, RoutedEventArgs e)
		{
			if (!File.Exists("./bin\\fpsunlocker.exe"))
			{
				using (WebClient webClient = new WebClient
				{
					Proxy = null
				})
				{
					webClient.DownloadFile("https://cdn.discordapp.com/attachments/1041093775671435335/1048686669634752512/rbxfpsunlocker.exe", "./bin\\fpsunlocker.exe");
				}
			}
			Process.Start("./bin\\fpsunlocker.exe");
		}

		void autoupdate_Copy_Unchecked(object sender, RoutedEventArgs e)
		{
			Process[] processesByName = Process.GetProcessesByName("fpsunlocker");
			for (int i = 0; i < processesByName.Length; i++)
			{
				processesByName[i].Kill();
			}
		}

		void autoexec_Copy_Checked(object sender, RoutedEventArgs e)
		{
			XWindow.<autoexec_Copy_Checked>d__78 <autoexec_Copy_Checked>d__;
			<autoexec_Copy_Checked>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<autoexec_Copy_Checked>d__.<>4__this = this;
			<autoexec_Copy_Checked>d__.<>1__state = -1;
			<autoexec_Copy_Checked>d__.<>t__builder.Start<XWindow.<autoexec_Copy_Checked>d__78>(ref <autoexec_Copy_Checked>d__);
		}

		void autoupdate5_Checked(object sender, RoutedEventArgs e)
		{
			XWindow.<autoupdate5_Checked>d__79 <autoupdate5_Checked>d__;
			<autoupdate5_Checked>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<autoupdate5_Checked>d__.<>4__this = this;
			<autoupdate5_Checked>d__.<>1__state = -1;
			<autoupdate5_Checked>d__.<>t__builder.Start<XWindow.<autoupdate5_Checked>d__79>(ref <autoupdate5_Checked>d__);
		}

		void autoexec5_Checked(object sender, RoutedEventArgs e)
		{
			XWindow.<autoexec5_Checked>d__80 <autoexec5_Checked>d__;
			<autoexec5_Checked>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<autoexec5_Checked>d__.<>4__this = this;
			<autoexec5_Checked>d__.<>1__state = -1;
			<autoexec5_Checked>d__.<>t__builder.Start<XWindow.<autoexec5_Checked>d__80>(ref <autoexec5_Checked>d__);
		}

		void Window_Activated(object sender, EventArgs e)
		{
			if (this.autofade.IsChecked.Value)
			{
				this.opacityanim(this.mwyes, base.Opacity, 1.0);
			}
		}

		void Window_Deactivated(object sender, EventArgs e)
		{
			if (this.autofade.IsChecked.Value)
			{
				this.opacityanim(this.mwyes, base.Opacity, 0.7);
			}
		}

		void opacity_Checked(object sender, RoutedEventArgs e)
		{
			this.opacityanim(this.mwyes, base.Opacity, 0.7);
		}

		void opacity_Unchecked(object sender, RoutedEventArgs e)
		{
			this.opacityanim(this.mwyes, base.Opacity, 1.0);
		}

		void topmost_Checked(object sender, RoutedEventArgs e)
		{
			base.Topmost = true;
		}

		void topmost_Unchecked(object sender, RoutedEventArgs e)
		{
			base.Topmost = false;
		}

		void Button_Click_5(object sender, RoutedEventArgs e)
		{
			try
			{
				ContentControl contentControl = this.search_filter_selection.SelectedItem as ComboBoxItem;
				string filters = "";
				string text = contentControl.Content as string;
				if (text != null)
				{
					if (!(text == "Hot"))
					{
						if (!(text == "Newest"))
						{
							if (!(text == "Oldest"))
							{
								if (text == "Most Viewed")
								{
									filters = "mostviewed";
								}
							}
							else
							{
								filters = "oldest";
							}
						}
						else
						{
							filters = "newest";
						}
					}
					else
					{
						filters = "hot";
					}
				}
				this.search_scriptblox(this.searchbox.Text, filters);
			}
			catch (Exception ex)
			{
				this.exceptionhandling(ex);
			}
		}

		void Button_Click_6(object sender, RoutedEventArgs e)
		{
			this.RunScript(this.GetText());
		}

		void Button_Click_7(object sender, RoutedEventArgs e)
		{
			try
			{
				this.Inject();
			}
			catch (Exception ex)
			{
				this.exceptionhandling(ex);
			}
		}

		public void exceptionhandling(Exception ex)
		{
			try
			{
				MessageBox.Show(ex.Message, "Delta Error");
				Directory.CreateDirectory(System.IO.Path.Combine(this.deltacore, "error_logs"));
				string text = System.IO.Path.Combine(System.IO.Path.Combine(new string[]
				{
					System.IO.Path.Combine(this.deltacore, "error_logs")
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
			catch
			{
				MessageBox.Show(ex.Message, "Delta Error");
				Directory.CreateDirectory(System.IO.Path.Combine(this.deltacore, "error_logs"));
				string text2 = System.IO.Path.Combine(System.IO.Path.Combine(new string[]
				{
					System.IO.Path.Combine(this.deltacore, "error_logs")
				}), "ERROR_LOG_" + DateTime.UtcNow.TimeOfDay.TotalMilliseconds.ToString() + ".txt");
				string contents2 = string.Format("GO TO OUR DISCORD MAKE A TICKET AND SEND THIS\r\n\r\nDelta Error Log {0}\r\n\r\nFile Name: {1}\r\nHResult: {2}\r\nException Data: {3}\r\nCausing Func: {4}\r\nTarget Site: {5}\r\nOuter Exception Message: {6}\r\nInner Exception Message: {7}\r\nOuter Exception Source: {8}\r\n\r\n\r\nException as String: {9}\r\n", new object[]
				{
					DateTime.Now,
					text2,
					ex.HResult,
					ex.Data,
					new StackTrace(ex, true).GetFrame(0).GetMethod().Name,
					ex.TargetSite,
					ex.Message,
					ex.InnerException.Message,
					ex.Source,
					ex
				});
				File.WriteAllText(text2, contents2);
				Process.Start(text2);
			}
		}

		void theme_Checked(object sender, RoutedEventArgs e)
		{
			if (this.theme_wrap_panel.Children.Count == 0)
			{
				this.setup_themes();
			}
		}

		void wrd_Checked(object sender, RoutedEventArgs e)
		{
			this.delta.IsChecked = new bool?(false);
		}

		void delta_Checked(object sender, RoutedEventArgs e)
		{
			this.wrd.IsChecked = new bool?(false);
		}

		void Button_Click_8(object sender, RoutedEventArgs e)
		{
			try
			{
				object arg = JsonConvert.DeserializeObject(File.ReadAllText(System.IO.Path.Combine(this.deltacore, "Settings.json")));
				foreach (object obj in ((IEnumerable)this.options.Items))
				{
					TabItem tabItem = (TabItem)obj;
					if (tabItem.Name != "langtab")
					{
						using (IEnumerator enumerator2 = (tabItem.Content as StackPanel).Children.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								CheckBox checkBox = (CheckBox)obj2;
								if (XWindow.<>o__94.<>p__5 == null)
								{
									XWindow.<>o__94.<>p__5 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(XWindow)));
								}
								foreach (object arg2 in XWindow.<>o__94.<>p__5.Target(XWindow.<>o__94.<>p__5, arg))
								{
									if (XWindow.<>o__94.<>p__3 == null)
									{
										XWindow.<>o__94.<>p__3 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(XWindow), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									Func<CallSite, object, bool> target = XWindow.<>o__94.<>p__3.Target;
									CallSite <>p__ = XWindow.<>o__94.<>p__3;
									if (XWindow.<>o__94.<>p__2 == null)
									{
										XWindow.<>o__94.<>p__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(XWindow), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
										}));
									}
									Func<CallSite, object, string, object> target2 = XWindow.<>o__94.<>p__2.Target;
									CallSite <>p__2 = XWindow.<>o__94.<>p__2;
									if (XWindow.<>o__94.<>p__1 == null)
									{
										XWindow.<>o__94.<>p__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(XWindow), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									Func<CallSite, object, object> target3 = XWindow.<>o__94.<>p__1.Target;
									CallSite <>p__3 = XWindow.<>o__94.<>p__1;
									if (XWindow.<>o__94.<>p__0 == null)
									{
										XWindow.<>o__94.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(XWindow), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
										}));
									}
									if (target(<>p__, target2(<>p__2, target3(<>p__3, XWindow.<>o__94.<>p__0.Target(XWindow.<>o__94.<>p__0, arg2, "CBName")), checkBox.Name)))
									{
										if (XWindow.<>o__94.<>p__4 == null)
										{
											XWindow.<>o__94.<>p__4 = CallSite<Func<CallSite, object, string, bool?, object>>.Create(Binder.SetIndex(CSharpBinderFlags.None, typeof(XWindow), new CSharpArgumentInfo[]
											{
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
											}));
										}
										XWindow.<>o__94.<>p__4.Target(XWindow.<>o__94.<>p__4, arg2, "CBState", checkBox.IsChecked);
									}
								}
							}
							continue;
						}
					}
					foreach (object obj3 in ((tabItem.Content as ScrollViewer).Content as StackPanel).Children)
					{
						CheckBox checkBox2 = (CheckBox)obj3;
						if (XWindow.<>o__94.<>p__11 == null)
						{
							XWindow.<>o__94.<>p__11 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(XWindow)));
						}
						foreach (object arg3 in XWindow.<>o__94.<>p__11.Target(XWindow.<>o__94.<>p__11, arg))
						{
							if (XWindow.<>o__94.<>p__9 == null)
							{
								XWindow.<>o__94.<>p__9 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(XWindow), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, object, bool> target4 = XWindow.<>o__94.<>p__9.Target;
							CallSite <>p__4 = XWindow.<>o__94.<>p__9;
							if (XWindow.<>o__94.<>p__8 == null)
							{
								XWindow.<>o__94.<>p__8 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(XWindow), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
								}));
							}
							Func<CallSite, object, string, object> target5 = XWindow.<>o__94.<>p__8.Target;
							CallSite <>p__5 = XWindow.<>o__94.<>p__8;
							if (XWindow.<>o__94.<>p__7 == null)
							{
								XWindow.<>o__94.<>p__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(XWindow), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, object, object> target6 = XWindow.<>o__94.<>p__7.Target;
							CallSite <>p__6 = XWindow.<>o__94.<>p__7;
							if (XWindow.<>o__94.<>p__6 == null)
							{
								XWindow.<>o__94.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(XWindow), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							if (target4(<>p__4, target5(<>p__5, target6(<>p__6, XWindow.<>o__94.<>p__6.Target(XWindow.<>o__94.<>p__6, arg3, "CBName")), checkBox2.Name)))
							{
								if (XWindow.<>o__94.<>p__10 == null)
								{
									XWindow.<>o__94.<>p__10 = CallSite<Func<CallSite, object, string, bool?, object>>.Create(Binder.SetIndex(CSharpBinderFlags.None, typeof(XWindow), new CSharpArgumentInfo[]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
									}));
								}
								XWindow.<>o__94.<>p__10.Target(XWindow.<>o__94.<>p__10, arg3, "CBState", checkBox2.IsChecked);
							}
						}
					}
				}
				if (XWindow.<>o__94.<>p__13 == null)
				{
					XWindow.<>o__94.<>p__13 = CallSite<Action<CallSite, Type, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "WriteAllText", null, typeof(XWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Action<CallSite, Type, string, object> target7 = XWindow.<>o__94.<>p__13.Target;
				CallSite <>p__7 = XWindow.<>o__94.<>p__13;
				Type typeFromHandle = typeof(File);
				string arg4 = System.IO.Path.Combine(this.deltacore, "Settings.json");
				if (XWindow.<>o__94.<>p__12 == null)
				{
					XWindow.<>o__94.<>p__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, typeof(XWindow), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				target7(<>p__7, typeFromHandle, arg4, XWindow.<>o__94.<>p__12.Target(XWindow.<>o__94.<>p__12, arg));
				MessageBox.Show(this.save_text, "Delta", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			catch
			{
			}
		}

		void Button_Click_9(object sender, RoutedEventArgs e)
		{
			try
			{
				bool flag = false;
				Theme theme = new Theme();
				foreach (object obj in this.theme_wrap_panel.Children)
				{
					ThemeItem themeItem = (ThemeItem)obj;
					if (themeItem.cyan.IsChecked.Value)
					{
						flag = true;
						theme = themeItem.this__theme;
						this.Overlay.Opacity = theme.opacity * 1.2;
					}
				}
				if (flag)
				{
					this.Overlay.Visibility = Visibility.Visible;
					this.img_theme.Source = new BitmapImage(new Uri(theme.imageURL, UriKind.Absolute));
					File.WriteAllText(this.deltacore + "\\themes.txt", string.Format("{0}", theme.id));
				}
			}
			catch (Exception ex)
			{
				this.exceptionhandling(ex);
			}
		}

		void Button_Click_10(object sender, RoutedEventArgs e)
		{
			this.Overlay.Visibility = Visibility.Hidden;
			File.WriteAllText(this.deltacore + "\\themes.txt", "0");
		}

		void opendllfolder_Checked(object sender, RoutedEventArgs e)
		{
			XWindow.<opendllfolder_Checked>d__97 <opendllfolder_Checked>d__;
			<opendllfolder_Checked>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<opendllfolder_Checked>d__.<>4__this = this;
			<opendllfolder_Checked>d__.<>1__state = -1;
			<opendllfolder_Checked>d__.<>t__builder.Start<XWindow.<opendllfolder_Checked>d__97>(ref <opendllfolder_Checked>d__);
		}

		void autoinj_Checked(object sender, RoutedEventArgs e)
		{
			this.DT.Start();
		}

		void autoinj_Unchecked(object sender, RoutedEventArgs e)
		{
			this.DT.Stop();
			this.DT1.Stop();
		}

		void mwyes_Closed(object sender, EventArgs e)
		{
			try
			{
				List<Tab> list = new List<Tab>();
				foreach (object obj in ((IEnumerable)this.tabControl1.Items))
				{
					TabItem tabItem = (TabItem)obj;
					list.Add(new Tab
					{
						title = (tabItem.Header as TextBox).Text,
						content = (tabItem.Content as TextEditor).Text
					});
				}
				File.WriteAllText(System.IO.Path.Combine(this.deltacore, "saved_tabs.json"), JsonConvert.SerializeObject(list));
				this.save_ui_data();
			}
			catch (Exception ex)
			{
				this.exceptionhandling(ex);
			}
		}

		void button1_Click(object sender, RoutedEventArgs e)
		{
			(((XWindow)Application.Current.MainWindow).TryFindResource("scriptdetailsback") as Storyboard).Begin();
			(((XWindow)Application.Current.MainWindow).TryFindResource("showsidebar") as Storyboard).Begin();
		}

		void search_filter_selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.load_favs();
		}

		public void load_favs()
		{
			try
			{
				if ((this.search_filter_selection.SelectedItem as ComboBoxItem).Name == "FAVS")
				{
					this.searchbox.Text = "";
					this.placeholdertext.Visibility = Visibility.Hidden;
					this.wrap_panel.Children.Clear();
					if (!File.Exists(System.IO.Path.Combine(this.deltacore, "saved_scripts")))
					{
						goto IL_173;
					}
					object arg = JsonConvert.DeserializeObject(File.ReadAllText(System.IO.Path.Combine(this.deltacore, "saved_scripts")));
					if (XWindow.<>o__103.<>p__0 == null)
					{
						XWindow.<>o__103.<>p__0 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(XWindow)));
					}
					using (IEnumerator enumerator = XWindow.<>o__103.<>p__0.Target(XWindow.<>o__103.<>p__0, arg).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (XWindow.<>o__103.<>p__1 == null)
							{
								XWindow.<>o__103.<>p__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(string), typeof(XWindow)));
							}
							string id = XWindow.<>o__103.<>p__1.Target(XWindow.<>o__103.<>p__1, enumerator.Current);
							this.wrap_panel.Children.Add(new ScriptBox(id, true, this.execute_again_button.Content.ToString()));
						}
						goto IL_173;
					}
				}
				this.placeholdertext.Visibility = Visibility.Visible;
				this.wrap_panel.Children.Clear();
				IL_173:;
			}
			catch (Exception ex)
			{
				this.exceptionhandling(ex);
			}
		}

		void button_Copy3_Checked(object sender, RoutedEventArgs e)
		{
			if (!this.is_liked)
			{
				this.is_liked = true;
				this.current_scriptbox.button.IsChecked = new bool?(true);
				this.like_count.Text = string.Format("{0}", int.Parse(this.like_count.Text) + 1);
			}
		}

		void button_Copy3_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.is_liked)
			{
				this.is_liked = false;
				this.current_scriptbox.button.IsChecked = new bool?(false);
				this.like_count.Text = string.Format("{0}", int.Parse(this.like_count.Text) - 1);
			}
		}

		void execute_again_button_Click(object sender, RoutedEventArgs e)
		{
			this.RunScript(this.execute_again_button.Tag.ToString());
		}

		void Button_Click_11(object sender, RoutedEventArgs e)
		{
			this.create_a_new_tab(this.script_title.Text + " description.txt", this.description_text.Text);
			this.home.IsChecked = new bool?(true);
		}

		void autoupdate1_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.englishlang();
		}

		void ger_deutsch_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.germanlang();
		}

		void ger_deutsch_Copy_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.turkishlang();
		}

		void spa_esp_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.spanishlang();
		}

		void ind_bah_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.indonesianlang();
		}

		void por_por_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.portlang();
		}

		void fil_pil_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.filipinolang();
		}

		void radioButton2_Checked(object sender, RoutedEventArgs e)
		{
		}

		void fre_fra_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.frenchlang();
		}

		void fin_sou_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.finnishlang();
		}

		void pol_pol_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.polishlang();
		}

		void Button_Click_12(object sender, RoutedEventArgs e)
		{
			this.create_a_new_tab(this.script_title.Text + ".lua", this.current_scriptbox.Data.script.script);
			this.home.IsChecked = new bool?(true);
		}

		void save_ui_data()
		{
			WindowSize value = new WindowSize(base.Width, base.Height);
			File.WriteAllText(System.IO.Path.Combine(this.deltacore, "windowsize.json"), JsonConvert.SerializeObject(value));
		}

		void load_ui_data()
		{
			if (File.Exists(System.IO.Path.Combine(this.deltacore, "windowsize.json")) && File.ReadAllText(System.IO.Path.Combine(this.deltacore, "windowsize.json")) != "[]" && XWindow.IsValidJson(File.ReadAllText(System.IO.Path.Combine(this.deltacore, "windowsize.json"))))
			{
				WindowSize windowSize = JsonConvert.DeserializeObject<WindowSize>(File.ReadAllText(System.IO.Path.Combine(this.deltacore, "windowsize.json")));
				base.Width = windowSize.width;
				base.Height = windowSize.height;
			}
		}

		void searchbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				try
				{
					ContentControl contentControl = this.search_filter_selection.SelectedItem as ComboBoxItem;
					string filters = "";
					string text = contentControl.Content as string;
					if (text != null)
					{
						if (!(text == "Hot"))
						{
							if (!(text == "Newest"))
							{
								if (!(text == "Oldest"))
								{
									if (text == "Most Viewed")
									{
										filters = "mostviewed";
									}
								}
								else
								{
									filters = "oldest";
								}
							}
							else
							{
								filters = "newest";
							}
						}
						else
						{
							filters = "hot";
						}
					}
					this.search_scriptblox(this.searchbox.Text, filters);
				}
				catch (Exception ex)
				{
					this.exceptionhandling(ex);
				}
			}
		}

		void thai_thai_Checked(object sender, RoutedEventArgs e)
		{
			this.uncheckalllangs(sender);
			this.thailang();
		}

		void execute_again_button_Copy_Click(object sender, RoutedEventArgs e)
		{
			File.Create(System.IO.Path.Combine(this.deltacore, "EULA.txt"));
			(base.TryFindResource("hide_eula") as Storyboard).Begin();
		}

		[DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 2)
			{
				((Button)target).Click += this.button_Click_1;
				return;
			}
			if (connectionId != 3)
			{
				return;
			}
			((Button)target).Click += this.Button_Click;
		}

		string no_results;

		string save_text;

		string auto_exec_prompt;

		public bool is_liked;

		public ScriptBox current_scriptbox;

		public string current_id;

		DispatcherTimer DT = new DispatcherTimer();

		DispatcherTimer DT1 = new DispatcherTimer();

		string deltacore = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "delta_core");

		string Module588 = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\588.dll";

		string Module587 = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\587.dll";

		string CurrentModule = "";

		public string rbxpath_lol;

		oxygenu_api _api = new oxygenu_api();

		DLls current_modules;
	}
}
