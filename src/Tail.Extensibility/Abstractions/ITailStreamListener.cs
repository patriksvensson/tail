using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
