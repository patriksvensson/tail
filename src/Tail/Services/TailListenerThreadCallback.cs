using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tail.Extensibility;

namespace Tail.Services
{
	internal sealed class TailListenerThreadCallback : ITailCallback
	{
		private readonly int _threadId;
		private readonly Action<int, string> _action;

		public TailListenerThreadCallback(int threadId, Action<int, string> action)
		{
			_threadId = threadId;
			_action = action;
		}

		public void Publish(string content)
		{
			_action(_threadId, content);
		}
	}
}
