using System;
using Caliburn.Micro;
using System.Linq;
using Tail.Tests.Fakes;
using Tail.ViewModels;
using Xunit;
using Moq;
using BlackBox;
using System.Reflection;
using System.Collections.Generic;
using Tail.Extensibility;

namespace Tail.Tests.Unit
{
	public class TailBootstrapperTests
	{
		#region Private Test Classes

		private class TestBootstrapper : TailBootstrapper
		{
			public TestBootstrapper(ITailProviderScannerService scanner)
				: base(scanner)
			{
			}

			protected override void StartRuntime()
			{
				Configure();
			}

			public bool HasBinding<T>()
			{
				return Kernel.GetBindings(typeof(T)).Any();
			}
		}

		#endregion

		[Fact]
		public void Should_Register_Event_Aggregator_In_Container()
		{
			// Given, When
			var scanner = new Mock<ITailProviderScannerService>().Object;
			var bootstrapper = new TestBootstrapper(scanner);

			// Then
			Assert.True(bootstrapper.HasBinding<IEventAggregator>());
		}

		[Fact]
		public void Should_Register_Window_Manager_In_Container()
		{
			// Given, When
			var scanner = new Mock<ITailProviderScannerService>().Object;
			var bootstrapper = new TestBootstrapper(scanner);

			// Then
			Assert.True(bootstrapper.HasBinding<IWindowManager>());
		}

		[Fact]
		public void Should_Register_Tail_Service_In_Container()
		{
			// Given, When
			var scanner = new Mock<ITailProviderScannerService>().Object;
			var bootstrapper = new TestBootstrapper(scanner);

			// Then
			Assert.True(bootstrapper.HasBinding<ITailListenerService>());
		}

		[Fact]
		public void Should_Register_Provider_Service_In_Container()
		{
			// Given, When
			var scanner = new Mock<ITailProviderScannerService>().Object;
			var bootstrapper = new TestBootstrapper(scanner);

			// Then
			Assert.True(bootstrapper.HasBinding<ITailProviderService>());
		}

		[Fact]
		public void Should_Register_Shell_View_Model_In_Container()
		{
			// Given, When
			var scanner = new Mock<ITailProviderScannerService>().Object;
			var bootstrapper = new TestBootstrapper(scanner);

			// Then
			Assert.True(bootstrapper.HasBinding<ShellViewModel>());
		}

		[Fact]
		public void Should_Register_Output_View_Model_In_Container()
		{
			// Given, When
			var scanner = new Mock<ITailProviderScannerService>().Object;
			var bootstrapper = new TestBootstrapper(scanner);

			// Then
			Assert.True(bootstrapper.HasBinding<StreamViewModel>());
		}

		[Fact]
		public void Should_Register_Log_Kernel_In_Container()
		{
			// Given, When
			var scanner = new Mock<ITailProviderScannerService>().Object;
			var bootstrapper = new TestBootstrapper(scanner);

			// Then
			Assert.True(bootstrapper.HasBinding<ILogKernel>());
		}

		[Fact]
		public void Should_Register_Found_Providers_In_Container()
		{
			// Given
			var scanner = new Mock<ITailProviderScannerService>();
			IEnumerable<Assembly> assemblies;
			var provider = new FakeProvider();
			var providers = new List<Type> { provider.GetType() };
			scanner.Setup(x => x.Scan(out assemblies)).Returns(() => providers);

			// When
			var bootstrapper = new TestBootstrapper(scanner.Object);

			// Then
			Assert.True(bootstrapper.HasBinding<ITailProvider>());
		}
	}
}
