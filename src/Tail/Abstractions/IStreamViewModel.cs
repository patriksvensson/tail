using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tail
{
	public interface IStreamViewModel
	{
		bool CanPause { get; }
		bool CanResume { get; }

		void Pause();
		void Resume();
	}
}
