namespace Tail.Extensibility
{
	public interface ITailSettingService
	{
		string Get(string name);
		void Set(string key, string value);
	}
}
