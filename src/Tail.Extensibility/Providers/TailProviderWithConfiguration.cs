using System;

namespace Tail.Extensibility
{
	public abstract class TailProviderWithConfiguration<TListener, TContext, TConfiguration> : ITailProvider
		where TListener : TailStreamListener<TContext>
		where TContext : ITailStreamContext
		where TConfiguration : ITailConfiguration
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
			get { return null; }
		}

		public abstract string GetDisplayName();
		public abstract ITailStreamContext CreateContext(TConfiguration configuration);

		ITailStreamContext ITailProvider.CreateContext(ITailConfiguration viewModel)
		{
			return CreateContext((TConfiguration)viewModel);
		}
	}
}