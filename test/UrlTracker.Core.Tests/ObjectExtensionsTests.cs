using System.Collections.Generic;
using NUnit.Framework;

namespace UrlTracker.Core.Tests
{
    public class ObjectExtensionsTests
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            return new TestCaseData[]
            {
                new TestCaseData("test", "test1", "test").SetName("input not null or whitespace returns input"),
                new TestCaseData(null, "test1", "test1").SetName("input null returns default"),
                new TestCaseData(string.Empty, "test1", "test1").SetName("input empty string returns default"),
            };
        }

        [TestCaseSource(nameof(TestCases))]
        public void DefaultIfNullOrWhiteSpace_NormalFlow_ReturnsCorrectValue(string input, string @default, string expected)
        {
            // arrange
            // act
            var result = input.DefaultIfNullOrWhiteSpace(@default);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
