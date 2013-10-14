using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
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
				this.NotifyOfPropertyChange(() => GlobalScope);
				this.Validate();
			}
		}

		public string ValidationError
		{
			get { return _validationError; }
			set
			{
				_validationError = value;
				this.NotifyOfPropertyChange(() => ValidationError);
			}
		}

		public bool Validate()
		{
			if (_globalScope)
			{
				if (!Win32Native.IsProcessElevated())
				{
					this.ValidationError = "Global scope requires administrator privileges.";
					return false;
				}
			}
			this.ValidationError = string.Empty;
			return true;
		}

		public ITailStreamContext GetContext()
		{
			return new DebugStreamContext(_globalScope);
		}
	}
}
