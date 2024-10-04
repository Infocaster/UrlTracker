using System.Threading.Tasks;
using NUnit.Framework;

namespace UrlTracker.Core.Caching.Memory.Tests
{
    public class TypedMemoryCacheTests
    {
        private const string _key = "test";
        private TypedMemoryCache<string, object>? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _testSubject = new TypedMemoryCache<string, object>(3);
        }

        [TearDown]
        public void TearDown()
        {
            _testSubject?.Dispose();
        }

        [TestCase(TestName = "Memory cache calls factory only once")]
        public async Task GetOrCreateAsync_NormalFlow_CallsFactoryOnce()
        {
            // arrange
            var expected = await _testSubject!.GetOrCreateAsync(_key, () => Task.FromResult(new object()));
            bool factoryCalled = false;

            // act
            var result = await _testSubject.GetOrCreateAsync("test", () => { factoryCalled = true; return Task.FromResult(new object()); });

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(factoryCalled, Is.False);
                Assert.That(result, Is.SameAs(expected));
            });
        }

        [TestCase(TestName = "Memory cache calls factory again after clear")]
        public async Task Clear_NormalFlow_ClearsCache()
        {
            // arrange
            const string Key = "test";
            var first = await _testSubject!.GetOrCreateAsync(Key, () => Task.FromResult(new object()));
            _testSubject.Clear();
            bool factoryCalled = false;

            // act
            var result = await _testSubject.GetOrCreateAsync(Key, () => { factoryCalled = true; return Task.FromResult(new object()); });

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(factoryCalled, Is.True);
                Assert.That(result, Is.Not.SameAs(first));
            });
        }
    }
}
