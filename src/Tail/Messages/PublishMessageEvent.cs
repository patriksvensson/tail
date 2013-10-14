using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tail.Messages
{
	public sealed class PublishMessageEvent
	{
		private readonly int _threadId;
		private readonly string _content;

		public int ThreadId
		{
			get { return _threadId; }
		}

		public string Content
		{
			get { return _content; }
		}

		public PublishMessageEvent(int threadId, string content)
		{
			if (threadId <= 0)
			{
				throw new ArgumentException(@"Thread ID must be greater than zero.", "threadId");
			}
			if (content == null)
			{
				content = string.Empty;
			}

			_threadId = threadId;
			_content = content;
		}
	}
}
