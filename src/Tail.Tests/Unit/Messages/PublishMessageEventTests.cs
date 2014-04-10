using System;
using Tail.Messages;
using Xunit;
using Xunit.Extensions;

namespace Tail.Tests.Unit.Messages
{
	public class PublishMessageEventTests
	{
		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Should_Throw_If_Thread_Id_Is_Invalid(int threadId)
		{
			// Given, When
			var result = Record.Exception(() => new PublishMessageEvent(threadId, "Hello World"));

			// Then
			Assert.IsType<ArgumentException>(result);
			Assert.Equal("threadId", ((ArgumentException)result).ParamName);
		}

		[Fact]
		public void Should_Set_Content_To_Empty_String_If_Null()
		{
			// Given, When
			var result = new PublishMessageEvent(1, null);

			// Then
			Assert.Equal(string.Empty, result.Content);
		}

		[Fact]
		public void Should_Set_Content()
		{
			// Given, When
			var result = new PublishMessageEvent(1, "Hello World");

			// Then
			Assert.Equal("Hello World", result.Content);
		}

		[Fact]
		public void Should_Set_Thread_ID()
		{
			// Given, When
			var result = new PublishMessageEvent(1, "Hello World");

			// Then
			Assert.Equal(1, result.ThreadId);
		}
	}
}
