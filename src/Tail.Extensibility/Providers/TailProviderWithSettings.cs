using System;

namespace Tail.Extensibility
{
	public abstract class TailProviderWithSettings<TListener, TContext, TSettings> : ITailProvider
		where TListener : TailStreamListener<TContext>
		where TContext : ITailStreamContext
		where TSettings : ITailSettings
	{
		public Type ConfigurationType
		{
			get { return null; }
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
		public abstract ITailStreamContext CreateContext();

		ITailStreamContext ITailProvider.CreateContext(ITailConfiguration viewModel)
		{
			return this.CreateContext();
		}
	}
}