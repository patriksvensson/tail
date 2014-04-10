using System;

namespace Tail.Extensibility
{
	public abstract class TailProviderWithConfigurationAndSettings<TListener, TContext, TConfiguration, TSettings> : ITailProvider
		where TListener : TailStreamListener<TContext>
		where TContext : ITailStreamContext
		where TConfiguration : ITailConfiguration
		where TSettings : ITailSettings
	{
		public Type ConfigurationType
		{
			get { return typeof(TConfiguration); }
		}

		public Type ListenerType
		{
			get { return typeof(TListener); }
		}

		public Type SettingsType
		{
			get { return typeof(TSettings); }
		}

		public abstract string GetDisplayName();
		public abstract ITailStreamContext CreateContext(TConfiguration configuration);

		ITailStreamContext ITailProvider.CreateContext(ITailConfiguration viewModel)
		{
			return CreateContext((TConfiguration)viewModel);
		}
	}
}