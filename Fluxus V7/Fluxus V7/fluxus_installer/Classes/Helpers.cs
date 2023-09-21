using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace fluxus_installer.Classes
{
	public static class Helpers
	{
		public static T GetTemplateItem<T>(this Control elem, string name)
		{
			object obj = elem.Template.FindName(name, elem);
			if (obj is T)
			{
				return (T)((object)obj);
			}
			return default(T);
		}

		public static Task beginAsync(this Storyboard storyBoard)
		{
			TaskCompletionSource<bool> status = new TaskCompletionSource<bool>();
			EventHandler sbHandler = null;
			sbHandler = delegate(object sender, EventArgs eeeeeeeeeeee)
			{
				storyBoard.Completed -= sbHandler;
				status.SetResult(true);
			};
			storyBoard.Completed += sbHandler;
			storyBoard.Begin();
			return status.Task;
		}

		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				int num;
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i = num + 1)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)((object)child);
					}
					foreach (T t in Helpers.FindVisualChildren<T>(child))
					{
						yield return t;
					}
					IEnumerator<T> enumerator = null;
					child = null;
					num = i;
				}
			}
			yield break;
			yield break;
		}
	}
}
