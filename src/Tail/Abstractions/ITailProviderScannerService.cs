using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tail
{
	public interface ITailProviderScannerService
	{
		IEnumerable<Type> Scan(out IEnumerable<Assembly> assemblies);
	}
}
