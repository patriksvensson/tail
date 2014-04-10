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
			Listen((TContext)context, callback, abortSignal);
		}

		void ITailStreamListener.Initialize(ITailStreamContext context)
		{
			Initialize((TContext)context);
		}
	}
}
