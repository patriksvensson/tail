using Caliburn.Micro;
using Tail.Messages;
using BlackBox;

namespace Tail.ViewModels
{
	public sealed class ShellViewModel : Conductor<IStreamViewModel>.Collection.OneActive, IShell,
		IHandle<StartListeningEvent>, IHandle<StopListeningEvent>,
		IHandle<StartedListeningEvent>, IHandle<StoppedListeningEvent>
	{
		private readonly IWindowManager _windowManager;
		private readonly ITailListenerService _service;
		private readonly ILogger _logger;
		private readonly IStreamConfigurationViewModelFactory _streamConfigurationFactory;
		private readonly IStreamViewModelFactory _streamFactory;
		private readonly ISettingsViewModelFactory _settingsFactory;
		private readonly AboutViewModel _aboutViewModel;
		private int _currentId;

		public bool CanPause
		{
			get { return ActiveItem != null && ActiveItem.CanPause; }
		}

		public bool CanResume
		{
			get { return ActiveItem != null && ActiveItem.CanResume; }
		}

		public bool CanOpenListener
		{
			get { return _currentId < 0; }
		}

		public bool CanCloseListener
		{
			get { return _currentId >= 0; }
		}

		public int CurrentId
		{
			get { return _currentId; }
			set
			{
				_currentId = value;
				NotifyOfPropertyChange(() => CanPause);
				NotifyOfPropertyChange(() => CanResume);
				NotifyOfPropertyChange(() => CanOpenListener);
				NotifyOfPropertyChange(() => CanCloseListener);
			}
		}

		public ShellViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, ITailListenerService service,
			ILogger logger, IStreamConfigurationViewModelFactory streamConfigurationFactory, IStreamViewModelFactory streamFactory,
			ISettingsViewModelFactory settingsFactory, AboutViewModel aboutViewModel)
		{
			// Subscribe for events.
            eventAggregator.Subscribe(this);

			_windowManager = windowManager;
			_service = service;
			_logger = logger;
			_streamConfigurationFactory = streamConfigurationFactory;
			_streamFactory = streamFactory;
			_settingsFactory = settingsFactory;
			_aboutViewModel = aboutViewModel;

			// Set the display name.
			DisplayName = "Tail";
			CurrentId = -1;
		}

	    protected override void OnDeactivate(bool close)
		{
			if (close)
			{
				_service.Stop();
			}
			base.OnDeactivate(close);
		}

		public void OpenListener()
		{
			var viewModel = _streamConfigurationFactory.Create();
			_windowManager.ShowDialog(viewModel);
		}

		public void CloseListener()
		{
			_logger.Information("Closing current listener");
			_service.Stop(_currentId);
		}

		public void Pause()
		{
			if (ActiveItem != null)
			{
				_logger.Information("Pausing auto scroll");
				ActiveItem.Pause();
			}
			NotifyOfPropertyChange(() => CanPause);
			NotifyOfPropertyChange(() => CanResume);
		}

		public void Resume()
		{
			if (ActiveItem != null)
			{
				_logger.Information("Resuming auto scroll");
				ActiveItem.Resume();
			}
			NotifyOfPropertyChange(() => CanPause);
			NotifyOfPropertyChange(() => CanResume);
		}

		public void ShowSettings()
		{
			var viewModel = _settingsFactory.Create();
			_windowManager.ShowDialog(viewModel);
		}

		public void ShowAbout()
		{
			_windowManager.ShowDialog(_aboutViewModel);
		}

	    public void Exit()
	    {
	        TryClose();
	    }

		public void Handle(StartListeningEvent message)
		{
			_logger.Information("Received StartListeningEvent ({0})", message.Context.GetDescription());
			_service.Start(message.Listener, message.Context);
		}

		public void Handle(StopListeningEvent message)
		{
			_logger.Information("Received StartListeningEvent (#{0})", message.ThreadId);

			if (message.ThreadId > 0)
			{
				_service.Stop(message.ThreadId);
			}
			else
			{
				_service.Stop();
			}
		}

		public void Handle(StartedListeningEvent message)
		{
			_logger.Information("Received StartedListeningEvent (#{0})", message.ThreadId);

			var viewModel = _streamFactory.Create(message.ThreadId);

			ActivateItem(viewModel);
			CurrentId = message.ThreadId;
			DisplayName = string.Format("Tail [{0}]", message.Description);
		}

		public void Handle(StoppedListeningEvent message)
		{
			_logger.Information("Received StoppedListeningEvent (#{0})", message.ThreadId);

			this.CloseItem(ActiveItem);

			CurrentId = -1;
			DisplayName = "Tail";
		}
	}
}
