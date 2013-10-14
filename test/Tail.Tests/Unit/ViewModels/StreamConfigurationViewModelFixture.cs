using Caliburn.Micro;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tail.Extensibility;
using Tail.Messages;
using Tail.Models;
using Tail.Tests.Fakes;
using Tail.ViewModels;
using Xunit;

namespace Tail.Tests.Unit.ViewModels
{
	public class StreamConfigurationViewModelFixture
	{
		public class TheConstructor
		{
			[Fact]
			public void Should_Create_Provider_Information_For_Each_Provider()
			{
				// Given
				var provider = new Mock<ITailProvider>();

				var service = new Mock<ITailProviderService>();
				service.Setup(x => x.GetProviderTypes()).Returns(() => new[] { provider.Object.GetType() });
				service.Setup(x => x.GetDisplayName(provider.Object.GetType())).Returns(() => "Test");

				// When
				var viewModel = new StreamConfigurationViewModel(service.Object, new Mock<IEventAggregator>().Object);

				// Then
				Assert.Equal(1, viewModel.Providers.Count);
				Assert.Equal("Test", viewModel.Providers[0].Name);
				Assert.Equal(provider.Object.GetType(), viewModel.Providers[0].Type);
			}

			[Fact]
			public void Should_Toggle_First_Provider_In_Provider_List()
			{
				// Given
				var configuration = new Mock<ITailConfiguration>();

				var service = new Mock<ITailProviderService>();
				service.Setup(x => x.GetProviderTypes()).Returns(() => new[] { typeof(string) });
				service.Setup(x => x.CreateConfigurationViewModel(typeof(string))).Returns(() => configuration.Object);
				service.Setup(x => x.GetDisplayName(typeof(string))).Returns(() => "Test");

				// When
				var viewModel = new StreamConfigurationViewModel(service.Object, new Mock<IEventAggregator>().Object);

				// Then
				Assert.Equal(configuration.Object, viewModel.ActiveItem);
			}
		}

		public class TheToggleProviderMethod
		{
			[Fact]
			public void Selected_Provider_Should_Be_The_Toggled_Provider()
			{
				// Given
				var configuration = new Mock<ITailConfiguration>();

				var service = new Mock<ITailProviderService>();
				service.Setup(x => x.GetProviderTypes()).Returns(() => new[] { typeof(string) });
				service.Setup(x => x.CreateConfigurationViewModel(typeof(string))).Returns(() => configuration.Object);
				service.Setup(x => x.GetDisplayName(typeof(string))).Returns(() => "Test");

				var viewModel = new StreamConfigurationViewModel(service.Object, new Mock<IEventAggregator>().Object);

				// When			
				viewModel.ToggleProvider(viewModel.Providers[0]);

				// Then
				Assert.Equal(viewModel.Providers[0], viewModel.SelectedProvider);
			}
		}

		public class TheOpenStreamMethod
		{
			[Fact]
			public void Should_Not_Open_Stream_If_No_Provider_Is_Selected()
			{
				bool called = false;

				// Given
				var provider = new FakeProviderService();

				var eventAggregator = new Mock<IEventAggregator>();
				eventAggregator.Setup(x => x.Publish(It.IsAny<StartListeningEvent>()))
					.Callback(() => called = true);

				var viewModel = new StreamConfigurationViewModel(provider, eventAggregator.Object);
				viewModel.SelectedProvider = null;

				// When
				viewModel.OpenStream();

				// Then
				Assert.False(called);
			}

			[Fact]
			public void Should_Not_Open_Stream_If_View_Model_Is_Invalid()
			{
				bool called = false;

				// Given
				var provider = new FakeProviderService();
				provider.RegisterProvider(typeof(FakeProvider));
				provider.RegisterConfiguration(typeof(FakeProvider), new FakeConfiguration(false));
				provider.RegisterContext(typeof(FakeProvider), new Mock<ITailStreamContext>().Object);

				var eventAggregator = new Mock<IEventAggregator>();
				eventAggregator.Setup(x => x.Publish(It.IsAny<StartListeningEvent>()))
					.Callback(() => called = true);

				var viewModel = new StreamConfigurationViewModel(provider, eventAggregator.Object);
				viewModel.SelectedProvider = new TailProviderInfo("Provider", typeof(FakeProvider));

				// When
				viewModel.OpenStream();

				// Then
				Assert.False(called);
			}

			[Fact]
			public void Should_Not_Open_Stream_If_Failing_To_Create_Listener()
			{
				bool called = false;

				// Given
				var provider = new FakeProviderService();
				provider.RegisterProvider(typeof(FakeProvider));
				provider.RegisterConfiguration(typeof(FakeProvider), new FakeConfiguration(true));
				provider.RegisterContext(typeof(FakeProvider), new Mock<ITailStreamContext>().Object);

				var eventAggregator = new Mock<IEventAggregator>();
				eventAggregator.Setup(x => x.Publish(It.IsAny<StartListeningEvent>()))
					.Callback(() => called = true);

				var viewModel = new StreamConfigurationViewModel(provider, eventAggregator.Object);
				viewModel.SelectedProvider = new TailProviderInfo("Provider", typeof(FakeProvider));

				// When
				viewModel.OpenStream();

				// Then
				Assert.False(called);
			}

			[Fact]
			public void Should_Not_Open_Stream_If_Failing_To_Create_Context()
			{
				bool called = false;

				// Given
				var provider = new FakeProviderService();
				provider.RegisterProvider(typeof(FakeProvider));
				provider.RegisterConfiguration(typeof(FakeProvider), new FakeConfiguration(true));
				provider.RegisterListener(typeof(FakeProvider), new Mock<ITailStreamListener>().Object);

				var eventAggregator = new Mock<IEventAggregator>();
				eventAggregator.Setup(x => x.Publish(It.IsAny<StartListeningEvent>()))
					.Callback(() => called = true);

				var viewModel = new StreamConfigurationViewModel(provider, eventAggregator.Object);
				viewModel.SelectedProvider = new TailProviderInfo("Provider", typeof(FakeProvider));

				// When
				viewModel.OpenStream();

				// Then
				Assert.False(called);
			}

			[Fact]
			public void Should_Send_Message_To_Open_Stream_If_Everything_Looks_Good()
			{
				bool called = false;

				// Given
				var provider = new FakeProviderService();
				provider.RegisterProvider(typeof(FakeProvider));
				provider.RegisterConfiguration(typeof(FakeProvider), new FakeConfiguration(true));
				provider.RegisterListener(typeof(FakeProvider), new Mock<ITailStreamListener>().Object);
				provider.RegisterContext(typeof(FakeProvider), new Mock<ITailStreamContext>().Object);

				var eventAggregator = new Mock<IEventAggregator>();
				eventAggregator.Setup(x => x.Publish(It.IsAny<StartListeningEvent>()))
					.Callback(() => called = true);

				var viewModel = new StreamConfigurationViewModel(provider, eventAggregator.Object);
				viewModel.SelectedProvider = new TailProviderInfo("Provider", typeof(FakeProvider));

				// When
				viewModel.OpenStream();

				// Then
				Assert.True(called);
			}

			[Fact]
			public void Should_Close_Window_After_Sending_Message()
			{
				bool called = false;

				// Given
				var provider = new FakeProviderService();
				provider.RegisterProvider(typeof(FakeProvider));
				provider.RegisterConfiguration(typeof(FakeProvider), new FakeConfiguration(true));
				provider.RegisterListener(typeof(FakeProvider), new Mock<ITailStreamListener>().Object);
				provider.RegisterContext(typeof(FakeProvider), new Mock<ITailStreamContext>().Object);

				var eventAggregator = new Mock<IEventAggregator>();
				eventAggregator.Setup(x => x.Publish(It.IsAny<StartListeningEvent>()))
					.Callback(() => called = true);

				var viewModel = new StreamConfigurationViewModel(provider, eventAggregator.Object);
				viewModel.SelectedProvider = new TailProviderInfo("Provider", typeof(FakeProvider));

				// When
				viewModel.OpenStream();

				// Then
				Assert.True(called);
				Assert.False(viewModel.IsActive);
			}
		}
	}
}
