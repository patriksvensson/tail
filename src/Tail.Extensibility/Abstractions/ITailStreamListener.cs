using System.Threading;

namespace Tail.Extensibility
{
	public interface ITailStreamListener
	{
		void Initialize(ITailStreamContext context);
		void Listen(ITailStreamContext context, ITailCallback callback, WaitHandle abortSignal);
		void Shutdown();
	}
}
