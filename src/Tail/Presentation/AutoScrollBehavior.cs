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
