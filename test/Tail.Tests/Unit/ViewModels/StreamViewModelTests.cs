using Caliburn.Micro;
using Moq;
using Tail.Messages;
using Tail.ViewModels;
using Xunit;

namespace Tail.Tests.Unit.ViewModels
{
	public class StreamViewModelTests
	{
		public class TheConstructor
		{
			[Fact]
			public void Should_Subscribe_To_Events()
			{
				// Given
				var eventAggregator = new Mock<IEventAggregator>();
				eventAggregator.Setup(x => x.Subscribe(It.IsAny<object>())).Verifiable("Did not subscribe to events.");

				// When
				new StreamViewModel(1, eventAggregator.Object);

				// Then
				eventAggregator.Verify();
			}

			[Fact]
			public void Should_Activate_Auto_Scrolling_By_Default()
			{
				// Given
				var eventAggregator = new Mock<IEventAggregator>();

				// When
				var viewModel = new StreamViewModel(1, eventAggregator.Object);

				// Then
				Assert.True(viewModel.AutoScrollEnabled);
			}
		}

		public class TheAutoScrollEnabledProperty
		{
			[Fact]
			public void Should_Notify_When_AutoScroll_Is_Changed()
			{
				var result = false;

				// Given
				var eventAggregator = new Mock<IEventAggregator>().Object;
				var viewModel = new StreamViewModel(1, eventAggregator);
				viewModel.PropertyChanged += (sender, args) => { result = true; };

				// When
				viewModel.AutoScrollEnabled = true;

				// Then
				Assert.True(result);
			}
		}


		public class ThePauseMethod
		{
			[Fact]
			public void Should_Disable_Auto_Scrolling_When_Paused()
			{
			    // Given
				var eventAggregator = new Mock<IEventAggregator>().Object;
				var viewModel = new StreamViewModel(1, eventAggregator);
				viewModel.AutoScrollEnabled = true;

				// When
				viewModel.Pause();

				// Then
				Assert.False(viewModel.AutoScrollEnabled);
			}
		}

		public class TheResumeMethod
		{
			[Fact]
			public void Should_Enable_Auto_Scrolling_When_Resumed()
			{
			    // Given
				var eventAggregator = new Mock<IEventAggregator>().Object;
				var viewModel = new StreamViewModel(1, eventAggregator);
				viewModel.AutoScrollEnabled = false;

				// When
				viewModel.Resume();

				// Then
				Assert.True(viewModel.AutoScrollEnabled);
			}
		}

		public class TheHandlePublishMessageMethod
		{
			[Fact]
			public void Should_Handle_Publish_Message_If_Message_Origins_From_Thread_With_Same_Id()
			{
				// Given
				var eventAggregator = new Mock<IEventAggregator>().Object;
				var viewModel = new StreamViewModel(1, eventAggregator);

				// When
				viewModel.Handle(new PublishMessageEvent(1, "Hello World"));

				// Then
				Assert.Equal("Hello World", viewModel.Output);
			}

			[Fact]
			public void Should_Ignore_Publish_Message_If_Message_Origins_From_Thread_With_Other_Id()
			{
				// Given
				var eventAggregator = new Mock<IEventAggregator>().Object;
				var viewModel = new StreamViewModel(1, eventAggregator);

				// When
				viewModel.Handle(new PublishMessageEvent(2, "Hello World"));

				// Then
				Assert.Equal(string.Empty, viewModel.Output);
			}
		}

		public class TheCanPauseProperty
		{
			[Fact]
			public void Can_Pause_If_Auto_Scroll_Is_Enabled()
			{
				// Given
				var eventAggregator = new Mock<IEventAggregator>().Object;
				var viewModel = new StreamViewModel(1, eventAggregator);

				// When
				viewModel.AutoScrollEnabled = true;

				// Then
				Assert.True(viewModel.CanPause);
			}
		}

		public class TheCanResumeProperty
		{
			[Fact]
			public void Can_Resume_If_Auto_Scroll_Is_Disabled()
			{
				// Given
				var eventAggregator = new Mock<IEventAggregator>().Object;
				var viewModel = new StreamViewModel(1, eventAggregator);

				// When
				viewModel.AutoScrollEnabled = false;

				// Then
				Assert.True(viewModel.CanResume);
			}
		}
	}
}
