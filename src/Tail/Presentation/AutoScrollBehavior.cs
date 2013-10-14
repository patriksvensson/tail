// ﻿
// Copyright (c) 2013 Patrik Svensson
// 
// This file is part of Tail.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
using System.Windows;
using System.Windows.Controls;

namespace Tail.Presentation
{
	public static class AutoScrollBehavior
	{
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached(
				"IsEnabled", typeof(bool), typeof(AutoScrollBehavior),
				new PropertyMetadata(false, OnScrollBehaviorChanged));

		public static bool GetIsEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(IsEnabledProperty, value);
		}

		private static void OnScrollBehaviorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var control = sender as TextBox;
			if (control != null)
			{
				if (e.NewValue is bool)
				{
					var enabled = (bool)(e.NewValue);
					if (enabled)
					{
						control.TextChanged += OnTextChanging;
					}
					else
					{
						control.TextChanged -= OnTextChanging;
					}
				}
			}
		}

		private static void OnTextChanging(object sender, TextChangedEventArgs e)
		{
			var control = e.Source as TextBox;
			if (control != null)
			{
				control.ScrollToEnd();
			}
		}
	}
}
