using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tail.Extensibility;

namespace Tail.Services
{
	internal sealed class TailProviderScannerService : ITailProviderScannerService
	{
		private readonly List<Assembly> _ignoredAssemblies;
		private readonly string _directory;

		public TailProviderScannerService()
		{
			_directory = Environment.CurrentDirectory;

			_ignoredAssemblies = new List<Assembly>();
			_ignoredAssemblies.Add(typeof(ITailProvider).Assembly);
		}

		public IEnumerable<Type> Scan(out IEnumerable<Assembly> assemblies)
		{
			// Create an own app domain for checking assemblies.
			var domain = AppDomain.CreateDomain("Tails-Providers-Temporary");

			try
			{
				// Make sure that the component directory exist.
				var directory = new DirectoryInfo(_directory);
				if (!directory.Exists)
				{
					assemblies = Enumerable.Empty<Assembly>();
					return Enumerable.Empty<Type>();
				}

				// Find all assemblies that contains components.
				var assemblyFiles = new List<FileInfo>();

				// Get all the assemblies in the directory.
				IEnumerable<FileInfo> files = GetFiles(directory);
				foreach (FileInfo file in files)
				{
					// Load the assembly into the temporary application domain.
					AssemblyName name = AssemblyName.GetAssemblyName(file.FullName);
					Assembly assembly = LoadAssembly(domain, name);
					if (assembly == null)
					{
						continue;
					}

					// Ignored?
					if (_ignoredAssemblies.Contains(assembly))
					{
						continue;
					}

					// Got any types here?
					var types = ScanAssembly(assembly);
					if (types.Any())
					{
						assemblyFiles.Add(file);
					}
				}

				var result = new List<Type>();
				var assemblyList = new List<Assembly>();

				// Load assemblies that contains the expected type into current domain.
				foreach (FileInfo assemblyFile in assemblyFiles)
				{
					AssemblyName name = AssemblyName.GetAssemblyName(assemblyFile.FullName);
					Assembly assembly = AppDomain.CurrentDomain.Load(name);

					assemblyList.Add(assembly);

					var components = ScanAssembly(assembly);
					foreach (var component in components)
					{
						result.Add(component);
					}
				}

				assemblies = assemblyList;

				return result;
			}
			finally
			{
				AppDomain.Unload(domain);
			}
		}

		private IEnumerable<FileInfo> GetFiles(DirectoryInfo directory)
		{
			var dlls = directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
			var exes = directory.GetFiles("*.exe", SearchOption.TopDirectoryOnly);
			return dlls.Union(exes);
		}

		private Assembly LoadAssembly(AppDomain domain, AssemblyName assemblyName)
		{
			try
			{
				return domain.Load(assemblyName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private IEnumerable<Type> ScanAssembly(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsClass && !type.IsAbstract)
				{
					if (typeof(ITailProvider).IsAssignableFrom(type))
					{
						yield return type;
					}
				}
			}
		}
	}
}
