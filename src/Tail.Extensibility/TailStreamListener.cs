using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tail.Extensibility
{
	public abstract class TailStreamListener<TContext> : ITailStreamListener
		where TContext : ITailStreamContext
	{
		public virtual void Initialize(TContext context)
		{
		}

		public virtual void Shutdown()
		{
		}

		public abstract void Listen(TContext context, ITailCallback callback, WaitHandle abortSignal);

		void ITailStreamListener.Listen(ITailStreamContext context, ITailCallback callback, WaitHandle abortSignal)
		{
			this.Listen((TContext)context, callback, abortSignal);
		}

		void ITailStreamListener.Initialize(ITailStreamContext context)
		{
			this.Initialize((TContext)context);
		}
	}
}
