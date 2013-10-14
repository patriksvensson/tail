using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Tail.Extensibility;
using System.IO;

namespace Tail.Providers.ViewModels
{
	internal class FileConfigurationViewModel : Screen, ITailConfiguration
	{
		private string _path;
		private string _validationError;

		public string Path
		{
			get { return _path; }
			set
			{
				_path = value;
				this.NotifyOfPropertyChange(() => Path);
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
			bool valid = !string.IsNullOrWhiteSpace(_path) && File.Exists(_path);
			if (!valid)
			{
				this.ValidationError = "File path is invalid.";
				return false;
			}
			this.ValidationError = string.Empty;
			return true;
		}

		public ITailStreamContext GetContext()
		{
			return new FileStreamContext(_path);
		}

		public void Browse()
		{
			var dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == true)
			{
				this.Path = dialog.FileName;
			}
			this.Validate();
		}
	}
}
