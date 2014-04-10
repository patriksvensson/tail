using BlackBox;
using Caliburn.Micro;
using Moq;
using Tail.ViewModels;
using Xunit;

namespace Tail.Tests.Unit.ViewModels
{
	public class ShellViewModelTests
	{
		public class TheConstructor
		{
			[Fact]
			public void Should_Subscribe_To_Events()
			{
				// Given
				var eventAggregator = new Mock<IEventAggregator>();
				eventAggregator.Setup(x => x.Subscribe(It.IsAny<object>())).Verifiable("Did not subscribe to events.");

				var windowManager = new Mock<IWindowManager>().Object;
				var listeningService = new Mock<ITailListenerService>().Object;
				var logger = new Mock<ILogger>().Object;
				var openStreamViewModelFactory = new Mock<IStreamConfigurationViewModelFactory>().Object;
				var streamViewModelFactory = new Mock<IStreamViewModelFactory>().Object;
				var settingsViewModelFactory = new Mock<ISettingsViewModelFactory>().Object;
				var viewModel = new AboutViewModel(new Mock<ILogger>().Object);

				// When
				new ShellViewModel(eventAggregator.Object, windowManager, listeningService, logger,
				    openStreamViewModelFactory, streamViewModelFactory, settingsViewModelFactory, viewModel);

				// Then
				eventAggregator.Verify();
			}
		}
	}
}
