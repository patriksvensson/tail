using Moq;
using System;
using Tail.Extensibility;
using Tail.Messages;
using Xunit;

namespace Tail.Tests.Unit.Messages
{
	public class StartListeningEventTests
	{
		[Fact]
		public void Should_Throw_If_Listener_Is_Null()
		{
			// Given
			var context = new Mock<ITailStreamContext>().Object;

			// When
			var result = Record.Exception(() => new StartListeningEvent(null, context));

			// Then
			Assert.IsType<ArgumentNullException>(result);
			Assert.Equal("listener", ((ArgumentNullException)result).ParamName);
		}

		[Fact]
		public void Should_Throw_If_Context_Is_Null()
		{
			// Given
			var listener = new Mock<ITailStreamListener>().Object;

			// When
			var result = Record.Exception(() => new StartListeningEvent(listener, null));

			// Then
			Assert.IsType<ArgumentNullException>(result);
			Assert.Equal("context", ((ArgumentNullException)result).ParamName);
		}

		[Fact]
		public void Should_Set_Listener()
		{
			// Given
			var listener = new Mock<ITailStreamListener>().Object;
			var context = new Mock<ITailStreamContext>().Object;

			// When
			var result = new StartListeningEvent(listener, context);

			// Then
			Assert.Equal(context, result.Context);
		}

		[Fact]
		public void Should_Set_Context()
		{
			// Given
			var listener = new Mock<ITailStreamListener>().Object;
			var context = new Mock<ITailStreamContext>().Object;

			// When
			var result = new StartListeningEvent(listener, context);

			// Then
			Assert.Equal(listener, result.Listener);
		}

	}
}
