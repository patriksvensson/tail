using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tail.Extensibility;

namespace Tail.Tests.Fakes
{
	public class FakeProviderService : ITailProviderService
	{
		private readonly List<Type> _providerTypes;
		private readonly Dictionary<Type, string> _displayNames;
		private readonly Dictionary<Type, ITailConfiguration> _configurationFactories;
		private readonly Dictionary<Type, ITailStreamListener> _listenerFactories;
		private readonly Dictionary<Type, ITailStreamContext> _contextFactories;

		public FakeProviderService()
		{
			_providerTypes = new List<Type>();
			_displayNames = new Dictionary<Type, string>();
			_configurationFactories = new Dictionary<Type, ITailConfiguration>();
			_listenerFactories = new Dictionary<Type, ITailStreamListener>();
			_contextFactories = new Dictionary<Type, ITailStreamContext>();
		}

		public void RegisterProvider(Type type)
		{
			_providerTypes.Add(type);
		}

		public void RegisterDisplayName(Type type, string displayName)
		{
			_displayNames.Add(type, displayName);
		}

		public void RegisterConfiguration(Type type, ITailConfiguration configuration)
		{
			_configurationFactories.Add(type, configuration);
		}

		public void RegisterListener(Type type, ITailStreamListener streamListener)
		{
			_listenerFactories.Add(type, streamListener);
		}

		public void RegisterContext(Type type, ITailStreamContext streamContext)
		{
			_contextFactories.Add(type, streamContext);
		}

		public Type[] GetProviderTypes()
		{
			return _providerTypes.ToArray();
		}

		public string GetDisplayName(Type providerType)
		{
			if (_displayNames.ContainsKey(providerType))
				return _displayNames[providerType];
			return null;
		}

		public ITailConfiguration CreateConfigurationViewModel(Type providerType)
		{
			if (_configurationFactories.ContainsKey(providerType))
				return _configurationFactories[providerType];
			return null;
		}

		public ITailStreamListener CreateListener(Type providerType)
		{
			if (_listenerFactories.ContainsKey(providerType))
				return _listenerFactories[providerType];
			return null;
		}

		public ITailStreamContext CreateContext(Type providerType, ITailConfiguration viewModel)
		{
			if (_contextFactories.ContainsKey(providerType))
				return _contextFactories[providerType];
			return null;
		}

		public ITailSettings CreateSettings(Type providerType)
		{
			throw new NotImplementedException();
		}
	}
}
