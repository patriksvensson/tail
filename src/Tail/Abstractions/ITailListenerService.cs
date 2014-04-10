using Tail.Extensibility;

namespace Tail
{
	public interface ITailListenerService
	{
		void Start(ITailStreamListener streamListener, ITailStreamContext streamContext);
		void Stop(int id);
		void Stop();
	}
}
