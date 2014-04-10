using Microsoft.Win32;
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
