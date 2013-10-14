using BlackBox;
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
using Caliburn.Micro;
using Moq;
using Tail.ViewModels;
using Xunit;

namespace Tail.Tests.Unit.ViewModels
{
	public class ShellViewModelFixture
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
				var shell = new ShellViewModel(eventAggregator.Object, windowManager, listeningService, logger,
					openStreamViewModelFactory, streamViewModelFactory, settingsViewModelFactory, viewModel);

				// Then
				eventAggregator.Verify();
			}
		}
	}
}
