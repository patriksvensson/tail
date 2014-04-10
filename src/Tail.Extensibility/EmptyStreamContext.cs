namespace Tail.Extensibility
{
	public sealed class EmptyStreamContext : ITailStreamContext
	{
		private readonly string _description;

		public string GetDescription()
		{
			return _description;
		}

		public EmptyStreamContext(string description)
		{
			_description = description;
		}
	}
}
