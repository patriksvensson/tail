using System;

namespace Tail.Messages
{
	public sealed class StartedListeningEvent
	{
		private readonly int _threadId;
		private readonly string _description;
	    private readonly string _tabName;

		public int ThreadId
		{
			get { return _threadId; }
		}

        public string TabName
        {
            get { return _tabName; }
        }

		public string Description
		{
			get { return _description; }
		}

		public StartedListeningEvent(int threadId, string description, string tabName)
		{
			if (threadId <= 0)
			{
				throw new ArgumentException(@"Thread ID must be greater than zero.", "threadId");
			}
			if (description == null)
			{
				throw new ArgumentNullException("description");
			}
			if (string.IsNullOrWhiteSpace(description))
			{
				throw new ArgumentException(@"Description cannot be empty.", "description");
			}
			_threadId = threadId;
			_description = description;
		    _tabName = tabName;
		}
	}
}
