using System;

namespace Tail.Models
{
	public sealed class TailProviderInfo
	{
		private readonly string _name;
		private readonly Type _type;

		public string Name
		{
			get { return _name; }
		}

		public Type Type
		{
			get { return _type; }
		}

		public TailProviderInfo(string name, Type type)
		{
			_name = name;
			_type = type;
		}
	}
}
