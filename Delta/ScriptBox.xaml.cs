using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;

namespace Delta
{
	public partial class ScriptBox : UserControl
	{
		public ScriptBox(string id, bool isfav, string exectext)
		{
			this.InitializeComponent();
			this.saved_id = id;
			this.realexectext = exectext;
			this.setup();
			this.is_liked = isfav;
		}

		public void setup()
		{
			ScriptBox.<setup>d__8 <setup>d__;
			<setup>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<setup>d__.<>4__this = this;
			<setup>d__.<>1__state = -1;
			<setup>d__.<>t__builder.Start<ScriptBox.<setup>d__8>(ref <setup>d__);
		}

		void Button_Click(object sender, RoutedEventArgs e)
		{
			((XWindow)Application.Current.MainWindow).RunScript(this.saved_script);
		}

		void button_Checked(object sender, RoutedEventArgs e)
		{
			this.Data.script.likeCount = this.Data.script.likeCount + 1;
			this.like_count.Text = this.Data.script.likeCount.ToString();
			if (!this.is_liked)
			{
				this.is_liked = true;
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "delta_core");
				List<string> list = new List<string>
				{
					this.Data.script.id ?? ""
				};
				if (File.Exists(Path.Combine(path, "saved_scripts")))
				{
					object arg = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(path, "saved_scripts")));
					if (ScriptBox.<>o__10.<>p__0 == null)
					{
						ScriptBox.<>o__10.<>p__0 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(ScriptBox)));
					}
					using (IEnumerator enumerator = ScriptBox.<>o__10.<>p__0.Target(ScriptBox.<>o__10.<>p__0, arg).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (ScriptBox.<>o__10.<>p__1 == null)
							{
								ScriptBox.<>o__10.<>p__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(string), typeof(ScriptBox)));
							}
							string item = ScriptBox.<>o__10.<>p__1.Target(ScriptBox.<>o__10.<>p__1, enumerator.Current);
							list.Add(item);
						}
					}
				}
				list.Remove(null);
				File.WriteAllText(Path.Combine(path, "saved_scripts"), JsonConvert.SerializeObject(list));
				if ((((XWindow)Application.Current.MainWindow).search_filter_selection.SelectedItem as ComboBoxItem).Name == "FAVS")
				{
					((XWindow)Application.Current.MainWindow).load_favs();
				}
			}
		}

		void button_Unchecked(object sender, RoutedEventArgs e)
		{
			this.Data.script.likeCount = this.Data.script.likeCount - 1;
			this.like_count.Text = this.Data.script.likeCount.ToString();
			if (this.is_liked)
			{
				this.is_liked = false;
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "delta_core");
				List<string> list = new List<string>();
				if (File.Exists(Path.Combine(path, "saved_scripts")))
				{
					object arg = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(path, "saved_scripts")));
					if (ScriptBox.<>o__11.<>p__0 == null)
					{
						ScriptBox.<>o__11.<>p__0 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(ScriptBox)));
					}
					using (IEnumerator enumerator = ScriptBox.<>o__11.<>p__0.Target(ScriptBox.<>o__11.<>p__0, arg).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (ScriptBox.<>o__11.<>p__1 == null)
							{
								ScriptBox.<>o__11.<>p__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(string), typeof(ScriptBox)));
							}
							string item = ScriptBox.<>o__11.<>p__1.Target(ScriptBox.<>o__11.<>p__1, enumerator.Current);
							list.Add(item);
						}
					}
					list.Remove(this.Data.script.id ?? "");
				}
				list.Remove(null);
				File.WriteAllText(Path.Combine(path, "saved_scripts"), JsonConvert.SerializeObject(list));
				if ((((XWindow)Application.Current.MainWindow).search_filter_selection.SelectedItem as ComboBoxItem).Name == "FAVS")
				{
					((XWindow)Application.Current.MainWindow).load_favs();
				}
			}
		}

		void Button_Click_1(object sender, RoutedEventArgs e)
		{
			string text = this.Data.script.game.imageUrl.ToString();
			if (text.Substring(0, 1) == "/")
			{
				text = "https://scriptblox.com" + text;
			}
			((XWindow)Application.Current.MainWindow).is_liked = this.is_liked;
			((XWindow)Application.Current.MainWindow).current_scriptbox = this;
			((XWindow)Application.Current.MainWindow).execute_again_button.Tag = this.Data.script.script;
			((XWindow)Application.Current.MainWindow).details_brush.ImageSource = LinkOpener.urltoimage(text);
			((XWindow)Application.Current.MainWindow).game_title.Text = this.Data.script.game.name;
			((XWindow)Application.Current.MainWindow).script_title.Text = this.Data.script.title;
			((XWindow)Application.Current.MainWindow).description_text.Text = this.Data.script.features;
			((XWindow)Application.Current.MainWindow).like_count.Text = this.Data.script.likeCount.ToString();
			((XWindow)Application.Current.MainWindow).dislike_count.Text = this.Data.script.dislikeCount.ToString();
			((XWindow)Application.Current.MainWindow).author_text.Text = this.Data.script.owner.username;
			((XWindow)Application.Current.MainWindow).button_Copy3.IsChecked = new bool?(this.is_liked);
			((XWindow)Application.Current.MainWindow).current_id = this.Data.script.id;
			(((XWindow)Application.Current.MainWindow).TryFindResource("scriptdetails") as Storyboard).Begin();
		}

		string saved_id;

		string realexectext;

		string saved_script;

		string real_script_string;

		public bool is_finished;

		public bool is_liked;

		public ScriptClass Data;
	}
}
