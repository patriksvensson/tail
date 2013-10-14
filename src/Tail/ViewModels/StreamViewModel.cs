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
using System.Windows;
using Caliburn.Micro;
using Tail.Messages;

namespace Tail.ViewModels
{
	/// <summary>
	/// This is a factory interface used by Ninject.
	/// Do not remove this even if it seem unused.
	/// </summary>
	public interface IStreamViewModelFactory
	{
		StreamViewModel Create(int id);
	}

	public sealed class StreamViewModel : Screen, IStreamViewModel, IHandle<PublishMessageEvent>
	{
		private readonly int _id;
		private readonly IEventAggregator _eventAggregator;
		private string _output;
		private bool _autoScrollEnabled;

		public string Output
		{
			get { return _output; }
			set
			{
				_output = value;
				this.NotifyOfPropertyChange(() => Output);
			}
		}

		public bool AutoScrollEnabled
		{
			get { return _autoScrollEnabled; }
			set
			{
				_autoScrollEnabled = value;
				this.NotifyOfPropertyChange(() => AutoScrollEnabled);
			}
		}

		public bool CanPause
		{
			get { return _autoScrollEnabled; }
		}

		public bool CanResume
		{
			get { return !_autoScrollEnabled; }
		}

		public StreamViewModel(int id, IEventAggregator eventAggregator)
		{
			_id = id;
			_eventAggregator = eventAggregator;
			_eventAggregator.Subscribe(this);
			_autoScrollEnabled = true;
			_output = string.Empty;
		}

		public void Pause()
		{
			this.AutoScrollEnabled = false;
		}

		public void Resume()
		{
			this.AutoScrollEnabled = true;
			this.NotifyOfPropertyChange(() => AutoScrollEnabled);
		}

		public void Handle(PublishMessageEvent message)
		{
			if (message.ThreadId == _id)
			{
				this.Output += message.Content;
			}
		}
	}
}
