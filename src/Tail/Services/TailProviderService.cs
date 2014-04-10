using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using Tail.Extensibility;

namespace Tail.Services
{
	internal sealed class TailProviderService : ITailProviderService
	{
		private readonly IKernel _kernel;
		private readonly ITailProvider[] _providers;

		public TailProviderService(IKernel kernel, IEnumerable<ITailProvider> providers)
		{
			_kernel = kernel;
			_providers = providers.ToArray();
		}

		public Type[] GetProviderTypes()
		{
			return _providers.Select(x => x.GetType()).ToArray();
		}

		public ITailConfiguration CreateConfigurationViewModel(Type providerType)
		{
			var provider = _providers.FirstOrDefault(x => x.GetType() == providerType);
			if (provider != null)
			{
				if (provider.ConfigurationType != null)
				{
					var configuration = _kernel.Get(provider.ConfigurationType) as ITailConfiguration;
					if (configuration != null)
					{
						return configuration;
					}
				}
			}
			return null;
		}

		public ITailStreamListener CreateListener(Type providerType)
		{
			var provider = _providers.FirstOrDefault(x => x.GetType() == providerType);
			if (provider != null)
			{
				var listener = _kernel.Get(provider.ListenerType) as ITailStreamListener;
				if (listener != null)
				{
					return listener;
				}
			}
			return null;
		}

		public ITailStreamContext CreateContext(Type providerType, ITailConfiguration viewModel)
		{
			var provider = _providers.FirstOrDefault(x => x.GetType() == providerType);
			if (provider != null)
			{
				return provider.CreateContext(viewModel);
			}
			return null;
		}

		public string GetDisplayName(Type providerType)
		{
			var provider = _providers.FirstOrDefault(x => x.GetType() == providerType);
			if (provider != null)
			{
				return provider.GetDisplayName();
			}
			return null;
		}

		public ITailSettings CreateSettings(Type providerType)
		{
			var provider = _providers.FirstOrDefault(x => x.GetType() == providerType);
			if (provider != null)
			{
				if (provider.SettingsType != null)
				{
					var settings = _kernel.Get(provider.SettingsType) as ITailSettings;
					if (settings != null)
					{
						return settings;
					}
				}
			}
			return null;
		}
	}
}
