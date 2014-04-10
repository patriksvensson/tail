using System;
using Tail.Extensibility;

namespace Tail
{
	public interface ITailProviderService
	{
		Type[] GetProviderTypes();
		string GetDisplayName(Type providerType);

		ITailConfiguration CreateConfigurationViewModel(Type providerType);
		ITailStreamListener CreateListener(Type providerType);
		ITailStreamContext CreateContext(Type providerType, ITailConfiguration viewModel);
		ITailSettings CreateSettings(Type providerType);
	}
}
