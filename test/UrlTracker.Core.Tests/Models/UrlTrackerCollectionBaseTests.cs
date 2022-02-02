using System.Collections.Generic;
using NUnit.Framework;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Tests.Models
{
    public class UrlTrackerCollectionBaseTests
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(new[] { new object() }, null, 1).SetName("Create creates a new collection with total equal to amount of given items by default.");
            yield return new TestCaseData(new[] { new object() }, 2, 2).SetName("Create creates new collection with total equal to given value.");
        }

        [TestCaseSource(nameof(TestCases))]
        public void Create_NormalFlow_CreatesCollection(object[] input, int? inputTotal, int expectedTotal)
        {
            // arrange

            // act
            var result = TestCollection.Create(input, inputTotal);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EquivalentTo(input));
                Assert.That(result.Total, Is.EqualTo(expectedTotal));
            });
        }
    }

    public class TestCollection
        : UrlTrackerCollectionBase<TestCollection, object>
    { }
}
