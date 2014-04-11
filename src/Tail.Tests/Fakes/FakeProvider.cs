using System;
using Tail.Extensibility;

namespace Tail.Tests.Fakes
{
	public class FakeProvider : ITailProvider
	{
		private class FakeStreamListener : ITailStreamListener
		{
			public void Initialize(ITailStreamContext context)
			{
			}

			public void Listen(ITailStreamContext context, ITailCallback callback, System.Threading.WaitHandle abortSignal)
			{
			}

			public void Shutdown()
			{
			}
		}

		private class FakeViewModel
		{
		}

		public Type ConfigurationType
		{
			get { return typeof(FakeViewModel); }
		}

		public Type SettingsType
		{
			get { return typeof(FakeViewModel); }
		}

		public Type ListenerType
		{
			get { return typeof(FakeStreamListener); }
		}

		public string GetDisplayName()
		{
			return "Fake";
		}

		public ITailStreamContext CreateContext(ITailConfiguration viewModel)
		{
			return new EmptyStreamContext("Fake", "Name");
		}
	}
}