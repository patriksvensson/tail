using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Tail.Extensibility
{
	public static class TailSettingServiceExtensions
	{
		public static T Get<T>(this ITailSettingService service, string name, T defaultValue = default(T))
		{
			var value = service.Get(name);
			if (value != null)
			{
				var converter = TypeDescriptor.GetConverter(typeof(T));
				return (T)converter.ConvertFromInvariantString(value);
			}
			return defaultValue;
		}

		public static void Set<T>(this ITailSettingService service, string name, T value)
		{
			var converter = TypeDescriptor.GetConverter(typeof(T));
			service.Set(name, converter.ConvertToInvariantString(value));
		}
	}
}
