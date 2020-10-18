using FluentAssertions;
using MarekMotykaBot.Services.Core;
using Xunit;

namespace MarekMotykaBot.Tests.Services
{
    public class RegexCheckerTests
    {
        [Theory]
        [InlineData("co")]
        [InlineData("COOOOOOOOO")]
        [InlineData("Cooo")]
        [InlineData("Cooooo????")]
        [InlineData("Cooooo!!!???")]
        [InlineData("Cooooo??!!")]
        [InlineData("cccccCoooO!!!")]
        [InlineData("....cooooo")]
        [InlineData("....   cooooo???!")]
        public void IsWhatWord_GivenWhatAlikeWord_ShouldReturnTrue(string message)
        {
            // When
            var result = RegexChecker.IsWhatWord(message);
            
            // Then
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("what")]
        [InlineData("coco")]
        [InlineData("coooi")]
        [InlineData("cock")]
        [InlineData("cosiestao")]
        [InlineData("taco")]
        [InlineData("taco?!?!?!")]
        [InlineData("...... co sie dzieje?")]
        public void IsWhatWord_GivenWhatNotAlikeWord_ShouldReturnFalse(string message)
        {
            // When
            var result = RegexChecker.IsWhatWord(message);
            
            // Then
            result.Should().BeFalse();
        }
    }
}
