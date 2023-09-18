﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Vega_X.Classes;

namespace VegaX.SpawnableSettings
{
	public partial class SColorButton : UserControl
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

		public SColorButton()
		{
			this.InitializeComponent();
			string str = HandleSettings.ReadSString("Accent_Color").Substring(3);
			this.ColorPreview.Background = (Brush)new BrushConverter().ConvertFromString("#7F" + str);
			this.ColorPreview.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF" + str);
		}

		Storyboard StoryBoard = new Storyboard();

		TimeSpan duration = TimeSpan.FromMilliseconds(500.0);

		TimeSpan duration2 = TimeSpan.FromMilliseconds(1000.0);
	}
}
