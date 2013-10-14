using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tail.Extensibility
{
	public interface ITailCallback
	{
		void Publish(string content);
	}
}
