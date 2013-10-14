using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tail.Extensibility
{
	public interface ITailProvider
	{
		Type ConfigurationType { get; }
		Type SettingsType { get; }
		Type ListenerType { get; }

		string GetDisplayName();
		ITailStreamContext CreateContext(ITailConfiguration viewModel);
	}
}
