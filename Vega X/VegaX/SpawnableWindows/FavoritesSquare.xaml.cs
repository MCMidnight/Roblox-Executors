using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Vega_X.Classes;

namespace VegaX.SpawnableWindows
{
	public partial class FavoritesSquare : UserControl
	{
		public FavoritesSquare()
		{
			this.InitializeComponent();
			string str = HandleSettings.ReadSString("Accent_Color").Substring(3);
			this.ExecuteScriptF.Background = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
		}
	}
}
