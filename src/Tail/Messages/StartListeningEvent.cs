using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tail.Extensibility;

namespace Tail.Messages
{
	public sealed class StartListeningEvent
	{
		private readonly ITailStreamListener _listener;
		private readonly ITailStreamContext _context;

		public ITailStreamListener Listener
		{
			get { return _listener; }
		}

		public ITailStreamContext Context
		{
			get { return _context; }
		}

		public StartListeningEvent(ITailStreamListener listener, ITailStreamContext context)
		{
			if (listener == null)
			{
				throw new ArgumentNullException("listener");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			_listener = listener;
			_context = context;
		}
	}
}
