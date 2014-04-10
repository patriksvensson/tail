using System;

namespace Tail.Extensibility
{
	public abstract class TailProvider<TListener, TContext> : ITailProvider
		where TListener : TailStreamListener<TContext>
		where TContext : ITailStreamContext
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
		public abstract TContext CreateContext();

		ITailStreamContext ITailProvider.CreateContext(ITailConfiguration viewModel)
		{
			return CreateContext();
		}
	}
}