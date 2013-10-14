// ﻿
// Copyright (c) 2013 Patrik Svensson
// 
// This file is part of Tail.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
using Caliburn.Micro;
using System;
using Tail.Extensibility;
using Tail.Services;
using System.Collections.Generic;
using System.Linq;
using Tail.Messages;
using BlackBox;

namespace Tail.ViewModels
{
	public sealed class ShellViewModel : Conductor<IStreamViewModel>.Collection.OneActive, IShell,
		IHandle<StartListeningEvent>, IHandle<StopListeningEvent>,
		IHandle<StartedListeningEvent>, IHandle<StoppedListeningEvent>
	{
		private readonly IEventAggregator _eventAggregator;
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
			get { return this.ActiveItem != null && this.ActiveItem.CanPause; }
		}

		public bool CanResume
		{
			get { return this.ActiveItem != null && this.ActiveItem.CanResume; }
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
				this.NotifyOfPropertyChange(() => CanPause);
				this.NotifyOfPropertyChange(() => CanResume);
				this.NotifyOfPropertyChange(() => CanOpenListener);
				this.NotifyOfPropertyChange(() => CanCloseListener);
			}
		}

		public ShellViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, ITailListenerService service,
			ILogger logger, IStreamConfigurationViewModelFactory streamConfigurationFactory, IStreamViewModelFactory streamFactory,
			ISettingsViewModelFactory settingsFactory, AboutViewModel aboutViewModel)
		{
			// Subscribe for events.
			_eventAggregator = eventAggregator;
			_eventAggregator.Subscribe(this);

			_windowManager = windowManager;
			_service = service;
			_logger = logger;
			_streamConfigurationFactory = streamConfigurationFactory;
			_streamFactory = streamFactory;
			_settingsFactory = settingsFactory;
			_aboutViewModel = aboutViewModel;

			// Set the display name.
			this.DisplayName = "Tail";
			this.CurrentId = -1;
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
			if (this.ActiveItem != null)
			{
				_logger.Information("Pausing auto scroll");
				this.ActiveItem.Pause();
			}
			this.NotifyOfPropertyChange(() => CanPause);
			this.NotifyOfPropertyChange(() => CanResume);
		}

		public void Resume()
		{
			if (this.ActiveItem != null)
			{
				_logger.Information("Resuming auto scroll");
				this.ActiveItem.Resume();
			}
			this.NotifyOfPropertyChange(() => CanPause);
			this.NotifyOfPropertyChange(() => CanResume);
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

			this.ActivateItem(viewModel);
			this.CurrentId = message.ThreadId;
			this.DisplayName = string.Format("Tail [{0}]", message.Description);
		}

		public void Handle(StoppedListeningEvent message)
		{
			_logger.Information("Received StoppedListeningEvent (#{0})", message.ThreadId);

			this.CloseItem(this.ActiveItem);

			this.CurrentId = -1;
			this.DisplayName = "Tail";
		}
	}
}
