using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Tail.Extensibility;

namespace Tail.Services
{
	internal sealed class TailSettingService : ITailSettingService
	{
		public string Get(string name)
		{
			var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\patriksvensson\\Tail");
			if (key != null)
			{
				return key.GetValue(name) as string;
			}
			return null;
		}

		public void Set(string name, string value)
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\patriksvensson\\Tail");
			if (key != null)
			{
				key.SetValue(name, value);
			}
		}
	}
}
