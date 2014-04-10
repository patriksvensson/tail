using System;
using Tail.Messages;
using Xunit;
using Xunit.Extensions;

namespace Tail.Tests.Unit.Messages
{
	public class StartedListeningEventTests
	{
		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public void Should_Throw_If_Thread_ID_Is_Invalid(int threadId)
		{
			// Given, When
			var result = Record.Exception(() => new StartedListeningEvent(threadId, "Description"));

			// Then
			Assert.IsType<ArgumentException>(result);
			Assert.Equal("threadId", ((ArgumentException)result).ParamName);
		}

		[Fact]
		public void Should_Throw_If_Description_Is_Null()
		{
			// Given, When
			var result = Record.Exception(() => new StartedListeningEvent(1, null));

			// Then
			Assert.IsType<ArgumentNullException>(result);
			Assert.Equal("description", ((ArgumentNullException)result).ParamName);
		}

		[Fact]
		public void Should_Throw_If_Description_Is_Empty()
		{
			// Given, When
			var result = Record.Exception(() => new StartedListeningEvent(1, string.Empty));

			// Then
			Assert.IsType<ArgumentException>(result);
			Assert.Equal("description", ((ArgumentException)result).ParamName);
		}

		[Fact]
		public void Should_Set_Description()
		{
			// Given, When
			var result = new StartedListeningEvent(1, "Description");

			// Then
			Assert.Equal("Description", result.Description);
		}

		[Fact]
		public void Should_Set_Thread_ID()
		{
			// Given, When
			var result = new StartedListeningEvent(1, "Description");

			// Then
			Assert.Equal(1, result.ThreadId);
		}
	}
}
