namespace Tail
{
	public interface IStreamViewModel
	{
		bool CanPause { get; }
		bool CanResume { get; }

		void Pause();
		void Resume();
	}
}
