using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tail.Extensibility
{
	public interface ITailSettings : IScreen
	{
		bool IsDirty { get; }
		bool Validate(out string error);

		void Load(ITailSettingService service);
		void Save(ITailSettingService service);
	}
}
