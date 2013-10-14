using Ninject;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tail.Extensibility;
using Tail.Providers.ViewModels;

namespace Tail.Providers
{
	internal sealed class FileProvider : TailProviderWithConfiguration<FileStreamListener, FileStreamContext, FileConfigurationViewModel>
	{
		public override string GetDisplayName()
		{
			return "File";
		}

		public override ITailStreamContext CreateContext(FileConfigurationViewModel configuration)
		{
			return new FileStreamContext(configuration.Path);
		}
	}
}
