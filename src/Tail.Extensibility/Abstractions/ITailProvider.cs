using System;

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
