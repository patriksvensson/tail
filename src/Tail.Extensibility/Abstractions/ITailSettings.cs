using Caliburn.Micro;

namespace Tail.Extensibility
{
	public interface ITailSettings : IScreen
	{
		bool IsDirty { get; }
		bool Validate(out string error);

		void Load(ITailSettingService service);
		void Save(ITailSettingService service);
	}
}
