using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Tail.Extensibility
{
	public interface ITailSettingService
	{
		string Get(string name);
		void Set(string key, string value);
	}
}
