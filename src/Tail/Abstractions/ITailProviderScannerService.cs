using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tail
{
	public interface ITailProviderScannerService
	{
		IEnumerable<Type> Scan(out IEnumerable<Assembly> assemblies);
	}
}
