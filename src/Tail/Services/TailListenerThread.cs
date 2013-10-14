using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tail.Extensibility;
using Tail.Messages;

namespace Tail.Services
{
	internal sealed class TailListenerThread
	{
		private readonly int _id;
		private readonly ITailCallback _callback;
		private readonly Thread _thread;
		private readonly ManualResetEvent _stopSignal;
		private readonly ManualResetEvent _stoppedSignal;
		private readonly ITailStreamListener _listener;
		private readonly ITailStreamContext _context;

		public int Id
		{
			get { return _id; }
		}

		public TailListenerThread(int id, ITailCallback callback, ITailStreamListener listener, ITailStreamContext context)
		{
			_id = id;
			_callback = callback;
			_listener = listener;
			_context = context;
			_stopSignal = new ManualResetEvent(false);
			_stoppedSignal = new ManualResetEvent(false);

			// Start the thread.
			_thread = new Thread(this.Execute);
			_thread.IsBackground = true;
			_thread.Start();
		}

		public WaitHandle Stop()
		{
			// Signal the thread to stop.
			_stopSignal.Set();

			// Return a wait handle.
			return _stoppedSignal;
		}

		private void Execute()
		{
			try
			{
				// Start listening.
				_listener.Listen(_context, _callback, _stopSignal);
			}
			catch
			{
				// Signal that the thread has been stopped.
				_stopSignal.Set();
			}
			finally
			{
				// Deinitialize the listener.
				_listener.Shutdown();

				// The thread has been stopped.
				_stoppedSignal.Set();
			}
		}
	}
}
