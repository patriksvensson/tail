namespace Tail.Extensibility
{
	public sealed class EmptyStreamContext : ITailStreamContext
	{
		private readonly string _description;
	    private readonly string _name;

        public string GetName()
        {
            return _name;
        }

		public string GetDescription()
		{
			return _description;
		}

	    public EmptyStreamContext(string description, string name)
		{
			_description = description;
	        _name = name;
		}
	}
}
