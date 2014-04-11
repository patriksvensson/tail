using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Tail.Messages;
using BlackBox;
using Tail.Models;
using System.Diagnostics;

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
	    private readonly List<ViewModelMapping> _mappings;

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
			get { return true; }
		}

		public bool CanCloseListener
		{
            get { return ActiveItem != null; }
		}

	    public bool HasListeners
	    {
            get { return ActiveItem != null; }
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
            _mappings = new List<ViewModelMapping>();

			// Set the display name.
			DisplayName = "Tail";
		}

	    public override void ActivateItem(IStreamViewModel item)
	    {
	        base.ActivateItem(item);

            NotifyOfPropertyChange(() => CanPause);
            NotifyOfPropertyChange(() => CanResume);
            NotifyOfPropertyChange(() => CanOpenListener);
            NotifyOfPropertyChange(() => CanCloseListener);
            NotifyOfPropertyChange(() => HasListeners);
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
		    CloseListener(ActiveItem);
		}
        
        public void CloseListener(IStreamViewModel sender)
	    {
            var mapping = _mappings.SingleOrDefault(x => x.ViewModel == sender);
            if (mapping != null)
            {
                _logger.Information("Closing current listener #{0}", mapping.ThreadId);
                _service.Stop(mapping.ThreadId);
            }		
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
            Debug.Assert(message.ThreadId > -1, "Invalid thread ID recieved.");
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
            Debug.Assert(message.ThreadId > -1, "Invalid thread ID recieved.");
			_logger.Information("Received StartedListeningEvent (#{0})", message.ThreadId);

            // Create the view model.
			var viewModel = _streamFactory.Create(message.ThreadId);
		    viewModel.DisplayName = message.TabName;

            // Add a view model mapping to be able to do thread lookup.
            // The reason for this is that we don't want to let the view model
            // handle it's own associated thread ID.
            _mappings.Add(new ViewModelMapping(viewModel, message.ThreadId));

            // Activate the new stream listener.
			ActivateItem(viewModel);

            NotifyOfPropertyChange(() => CanPause);
            NotifyOfPropertyChange(() => CanResume);
            NotifyOfPropertyChange(() => CanOpenListener);
            NotifyOfPropertyChange(() => CanCloseListener);
            NotifyOfPropertyChange(() => HasListeners);
		}

		public void Handle(StoppedListeningEvent message)
		{
            Debug.Assert(message.ThreadId > -1, "Invalid thread ID recieved.");
			_logger.Information("Received StoppedListeningEvent (#{0})", message.ThreadId);

            // Find the view model mapping from the thread ID.
		    var mapping = _mappings.SingleOrDefault(x => x.ThreadId == message.ThreadId);
		    if (mapping != null)
		    {
		        var removed = _mappings.Remove(mapping);
                Debug.Assert(removed, "Could not remove view model mapping.");
                this.CloseItem(mapping.ViewModel);   
		    }

            NotifyOfPropertyChange(() => CanPause);
            NotifyOfPropertyChange(() => CanResume);
            NotifyOfPropertyChange(() => CanOpenListener);
            NotifyOfPropertyChange(() => CanCloseListener);
            NotifyOfPropertyChange(() => HasListeners);
		}
	}
}
