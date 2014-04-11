using Tail.Extensibility;

namespace Tail.Providers
{
	internal sealed class FileStreamContext : ITailStreamContext
	{
		private readonly string _path;

		public string Path
		{
			get { return _path; }
		}

		public string GetDescription()
		{
			return Path;
		}

        public string GetName()
        {
            return System.IO.Path.GetFileName(Path);
        }

		public FileStreamContext(string path)
		{
			_path = path;
		}
	}
}
