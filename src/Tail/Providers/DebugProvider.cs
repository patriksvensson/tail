using Tail.Extensibility;
using Tail.Providers.ViewModels;

namespace Tail.Providers
{
	internal sealed class DebugProvider : TailProviderWithConfiguration<DebugStreamListener, DebugStreamContext, DebugConfigurationViewModel>
	{
		public override string GetDisplayName()
		{
			return "Debug";
		}

		public override ITailStreamContext CreateContext(DebugConfigurationViewModel configuration)
		{
			return new DebugStreamContext(configuration.GlobalScope);
		}
	}
}
