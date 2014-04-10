using Caliburn.Micro;
using Tail.Providers.Utilities;
using Tail.Extensibility;

namespace Tail.Providers.ViewModels
{
	internal sealed class DebugConfigurationViewModel : Screen, ITailConfiguration
	{
		private bool _globalScope;
		private string _validationError;

		public bool GlobalScope
		{
			get { return _globalScope; }
			set
			{
				_globalScope = value;
				NotifyOfPropertyChange(() => GlobalScope);
				Validate();
			}
		}

		public string ValidationError
		{
			get { return _validationError; }
			set
			{
				_validationError = value;
				NotifyOfPropertyChange(() => ValidationError);
			}
		}

		public bool Validate()
		{
			if (_globalScope)
			{
				if (!Win32Native.IsProcessElevated())
				{
					ValidationError = "Global scope requires administrator privileges.";
					return false;
				}
			}
			ValidationError = string.Empty;
			return true;
		}

		public ITailStreamContext GetContext()
		{
			return new DebugStreamContext(_globalScope);
		}
	}
}
