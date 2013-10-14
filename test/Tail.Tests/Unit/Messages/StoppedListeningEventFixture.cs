using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tail.Messages;
using Xunit;
using Xunit.Extensions;

namespace Tail.Tests.Unit.Messages
{
	public class StoppedListeningEventFixture
	{
		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Should_Throw_If_Thread_ID_Is_Invalid(int threadId)
		{
			// Given, When
			var result = Record.Exception(() => new StoppedListeningEvent(threadId));

			// Then
			Assert.IsType<ArgumentException>(result);
			Assert.Equal("threadId", ((ArgumentException)result).ParamName);
		}

		[Fact]
		public void Should_Set_Thread_ID()
		{
			// Given, When
			var result = new StoppedListeningEvent(1);

			// Then
			Assert.Equal(1, result.ThreadId);
		}
	}
}
