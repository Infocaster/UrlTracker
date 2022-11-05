using System;
using System.Collections.Generic;
using NUnit.Framework;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Map;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests.Map
{
    public class StrategyMapBaseTests : TestBase
    {
        private TestStrategyMap _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new TestStrategyMap();
        }

        public static IEnumerable<TestCaseData> CanHandleComplexTestCaseSource()
        {
            yield return new TestCaseData(new UrlTargetStrategy("http://example.com"), false)
                .SetName("CanHandle returns false if target strategy is not a supported strategy");

            yield return new TestCaseData(new ContentPageTargetStrategy(default, default), true)
                .SetName("CanHandle returns true if target strategy is a supported strategy");

            yield return new TestCaseData(new MisleadingTargetStrategy(), false)
                .SetName("CanHandle returns false if target strategy has right key but not right type");
        }

        [TestCaseSource(nameof(CanHandleComplexTestCaseSource))]
        public void CanHandleComplex_SeveralStrategies_ReturnsCorrectResult(IStrategyBase strategy, bool expected)
        {
            // arrange

            // act
            var result = _testSubject.CanHandle(strategy);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<TestCaseData> CanHandleSimpleTestCaseSource()
        {
            yield return new TestCaseData(EntityStrategy.ContentTargetStrategy("1234"), true)
                .SetName("CanHandle returns true if target strategy is supported");

            yield return new TestCaseData(EntityStrategy.UrlTargetStrategy("http://example.com"), false)
                .SetName("CanHandle returns false if target strategy is not supported");
        }

        [TestCaseSource(nameof(CanHandleSimpleTestCaseSource))]
        public void CanHandleSimple_SeveralStrategies_ReturnsCorrectResult(EntityStrategy strategy, bool expected)
        {
            // arrange

            // act
            var result = _testSubject.CanHandle(strategy);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        public class TestStrategyMap : StrategyMapBase<ContentPageTargetStrategy>
        {
            protected override Guid StrategyKey => UrlTracker.Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content;

            public override ContentPageTargetStrategy Convert(EntityStrategy strategy)
            {
                throw new NotImplementedException();
            }

            protected override EntityStrategy Convert(ContentPageTargetStrategy strategy)
            {
                throw new NotImplementedException();
            }
        }

        private class MisleadingTargetStrategy : ITargetStrategy
        {
            public Guid Strategy => UrlTracker.Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content;
        }
    }
}
