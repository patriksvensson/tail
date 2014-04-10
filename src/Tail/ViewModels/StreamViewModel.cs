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
		private string _output;
		private bool _autoScrollEnabled;

		public string Output
		{
			get { return _output; }
			set
			{
				_output = value;
				NotifyOfPropertyChange(() => Output);
			}
		}

		public bool AutoScrollEnabled
		{
			get { return _autoScrollEnabled; }
			set
			{
				_autoScrollEnabled = value;
				NotifyOfPropertyChange(() => AutoScrollEnabled);
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
			_autoScrollEnabled = true;
			_output = string.Empty;

            // Subscribe to events.
            eventAggregator.Subscribe(this);
		}

		public void Pause()
		{
			AutoScrollEnabled = false;
		}

		public void Resume()
		{
			AutoScrollEnabled = true;
			NotifyOfPropertyChange(() => AutoScrollEnabled);
		}

		public void Handle(PublishMessageEvent message)
		{
			if (message.ThreadId == _id)
			{
				Output += message.Content;
			}
		}
	}
}
