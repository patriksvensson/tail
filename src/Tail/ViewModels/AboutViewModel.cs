using System.Diagnostics;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tail.Properties;
using BlackBox;

namespace Tail.ViewModels
{
	/// <summary>
	/// This is a factory interface used by Ninject.
	/// Do not remove this even if it seem unused.
	/// </summary>
	public interface IAboutViewModelFactory
	{
		AboutViewModel Create();
	}

	public class AboutViewModel : Screen
	{
		private readonly string _information;
		private readonly ILogger _logger;

		public string Information
		{
			get { return _information; }
		}

		public AboutViewModel(ILogger logger)
		{
			_information = Resources.Information;
			_logger = logger;

			this.DisplayName = "About Tail";
		}

		public void NavigateToGithub()
		{
			try
			{
				const string url = "https://github.com/patriksvensson/tail";
				Process.Start(new ProcessStartInfo(url));
			}
			catch (Exception ex)
			{
				_logger.Error("Could not navigate to Github URL: {0}", ex.Message);
			}
		}
	}
}
