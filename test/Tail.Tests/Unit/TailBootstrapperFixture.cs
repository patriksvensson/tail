// ﻿
// Copyright (c) 2013 Patrik Svensson
// 
// This file is part of Tail.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 

using System;
using Caliburn.Micro;
using System.Linq;
using Tail.Services;
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
	public class TailBootstrapperFixture
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
				this.Configure();
			}

			public bool HasBinding<T>()
			{
				return this.Kernel.GetBindings(typeof(T)).Any();
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
			var assemblies = Enumerable.Empty<Assembly>();
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
