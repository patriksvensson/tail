namespace Tail.Models
{
    public class ViewModelMapping
    {
        private IStreamViewModel _viewModel;
        private int _threadId;

        public IStreamViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public int ThreadId
        {
            get { return _threadId; }
        }

        public ViewModelMapping(IStreamViewModel viewModel, int threadId)
        {
            _viewModel = viewModel;
            _threadId = threadId;
        }
    }
}
