using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tail.Extensibility;
using Tail.Models;

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
