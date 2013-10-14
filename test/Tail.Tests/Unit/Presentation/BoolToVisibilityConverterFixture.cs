using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Tail.Extensibility;
using Tail.Presentation;
using Xunit;
using Xunit.Extensions;

namespace Tail.Tests.Unit.Presentation
{
	public class BoolToVisibilityConverterFixture
	{
		[Theory]
		[InlineData(true, Visibility.Visible)]
		[InlineData(false, Visibility.Collapsed)]
		public void Can_Convert_From_Boolean_To_Visibility(bool value, Visibility expected)
		{
			// Given
			var converter = new BoolToVisibilityConverter();

			// When
			var result = converter.Convert(value, null, null, null);

			// Then
			Assert.NotNull(result);
			Assert.IsType<Visibility>(result);
			Assert.Equal(expected, (Visibility)result);
		}

		[Fact]
		public void Should_Return_Null_If_Converting_From_Boolean_To_Anything_Other_Than_Visibility()
		{
			// Given
			var converter = new BoolToVisibilityConverter();

			// When
			var result = converter.Convert("Hello", null, null, null);

			// Then
			Assert.Null(result);
		}

		[Theory]
		[InlineData(Visibility.Visible, true)]
		[InlineData(Visibility.Collapsed, false)]
		[InlineData(Visibility.Hidden, false)]
		public void Can_Convert_From_Visibility_To_Boolean(Visibility value, bool expected)
		{
			// Given
			var converter = new BoolToVisibilityConverter();

			// When
			var result = converter.ConvertBack(value, null, null, null);

			// Then
			Assert.NotNull(result);
			Assert.IsType<bool>(result);
			Assert.Equal(expected, (bool)result);
		}

		[Fact]
		public void Should_Return_Null_If_Converting_From_Unknown_Visibility_Value()
		{
			// Given
			var converter = new BoolToVisibilityConverter();

			// When
			var result = converter.ConvertBack((Visibility)255, null, null, null);

			// Then
			Assert.Null(result);
		}

		[Fact]
		public void Should_Return_Null_If_Converting_From_Visibility_To_Anything_Other_Than_Boolean()
		{
			// Given
			var converter = new BoolToVisibilityConverter();

			// When
			var result = converter.ConvertBack("Hello", null, null, null);

			// Then
			Assert.Null(result);
		}
	}
}
