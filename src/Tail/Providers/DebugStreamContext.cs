using Tail.Extensibility;

namespace Tail.Providers
{
	internal sealed class DebugStreamContext : ITailStreamContext
	{
		private readonly bool _globalScope;

		public bool GlobalScope
		{
			get { return _globalScope; }
		}

		public DebugStreamContext(bool globalScope)
		{
			_globalScope = globalScope;
		}

		public string GetDescription()
		{
			return "Debug";
		}
	}
}
