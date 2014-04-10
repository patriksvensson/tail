using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tail.Presentation
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				if (((Visibility)value) == Visibility.Visible)
				{
					return true;
				}
				if (((Visibility)value) == Visibility.Collapsed)
				{
					return false;
				}
				if (((Visibility)value) == Visibility.Hidden)
				{
					return false;
				}
			}
			return null;
		}
	}
}
