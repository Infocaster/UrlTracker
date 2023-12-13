using NUnit.Framework;

namespace UrlTracker.Core.Tests
{
    public class StringExtensionsTests
    {
        [TestCase(null, null, TestName = "NormalizeCulture returns null if input is null")]
        [TestCase("NL", "NL", TestName = "NormalizeCulture returns original string if input contains no dash")]
        [TestCase("nl-nl", "nl-NL", TestName = "NormalizeCulture returns input string with segment behind dash in uppercase")]
        [TestCase("nl-NL", "nl-NL", TestName = "NormalizeCulture returns input string if segment behind dash is already uppercase")]
        public void NormalizeCulture_NormalFlow_ReturnsProperResult(string? input, string? expectedOutput)
        {
            // arrange

            // act
            var result = StringExtensions.NormalizeCulture(input);

            // assert
            Assert.That(result, Is.EqualTo(expectedOutput));
        }
    }
}
