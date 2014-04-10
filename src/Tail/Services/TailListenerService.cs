using BlackBox;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tail.Extensibility;
using Tail.Messages;

namespace Tail.Services
{
	internal sealed class TailListenerService : ITailListenerService
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly ILogger _logger;
		private readonly List<TailListenerThread> _threads;
		private readonly object _lock;
		private int _counter;

		public TailListenerService(IEventAggregator eventAggregator, ILogger logger)
		{
			_eventAggregator = eventAggregator;
			_logger = logger;
			_threads = new List<TailListenerThread>();
			_lock = new object();
			_counter = 0;
		}

		public void Start(ITailStreamListener streamListener, ITailStreamContext streamContext)
		{
			lock (_lock)
			{
				// Create the thread.
				var id = Interlocked.Increment(ref _counter);

				// Notify about a new thread.
				_eventAggregator.Publish(new StartedListeningEvent(id, streamContext.GetDescription()));

				// Initialize the listener.
				streamListener.Initialize(streamContext);

				// Create the thread.
				var callback = new TailListenerThreadCallback(id, Publish);
				var thread = new TailListenerThread(id, callback, streamListener, streamContext);

                
                _logger.Information("Created thread with id #{0} ({1}).", id, streamContext.GetDescription());

				// Add the thread to the collection.
				_threads.Add(thread);
			}
		}

		public void Stop()
		{
			lock (_lock)
			{
				_logger.Verbose("Stopping all threads.");

				foreach (var thread in _threads)
				{
					Stop(thread, remove: false);
				}
				_threads.Clear();
			}
		}

		public void Stop(int id)
		{
			lock (_lock)
			{
				var thread = _threads.FirstOrDefault(x => x.Id == id);
				if (thread != null)
				{
					// Stop the thread.
					Stop(thread);
				}
			}
		}

		private void Stop(TailListenerThread thread, bool remove = true)
		{
			if (thread != null)
			{
				_logger.Verbose("Stopping thread #{0}...", thread.Id);

				// Stop the thread.
				var handle = thread.Stop();

				// Wait for the thread to stop.
				_logger.Verbose("Waiting for thread #{0} to stop...", thread.Id);
				handle.WaitOne();

				if (remove)
				{
					// Remove the thread from the collection.
					_logger.Verbose("Removing thread #{0}...", thread.Id);
					_threads.Remove(thread);
				}

				_logger.Verbose("Thread #{0} was stopped.", thread.Id);

				// Notify that the thread was stopped.
				_eventAggregator.Publish(new StoppedListeningEvent(thread.Id));
			}
		}

		private void Publish(int threadId, string content)
		{
			_eventAggregator.Publish(new PublishMessageEvent(threadId, content));
		}
	}
}
