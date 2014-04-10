using Caliburn.Micro;

namespace Tail.Extensibility
{
	public interface ITailConfiguration : IScreen
	{
		bool Validate();
	}
}
