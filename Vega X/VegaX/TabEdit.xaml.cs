using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace VegaX
{
	public partial class TabEdit : Window
	{
		public TabEdit(TabControl tabs)
		{
			this.InitializeComponent();
			this.tabs = tabs;
			base.Topmost = true;
			base.Deactivated += delegate(object sender, EventArgs e)
			{
				base.Hide();
			};
		}

		public void Show(TabItem tab)
		{
			this.tab = tab;
			base.Show();
		}

		void Rename_Click(object sender, RoutedEventArgs e)
		{
			base.Hide();
			TextBox textBox = this.tab.Header as TextBox;
			textBox.IsEnabled = true;
			textBox.Focus();
			textBox.SelectAll();
		}

		void Close_Click(object sender, RoutedEventArgs e)
		{
			base.Hide();
			this.tabs.Items.Remove(this.tab);
		}

		TabControl tabs;

		TabItem tab;
	}
}
