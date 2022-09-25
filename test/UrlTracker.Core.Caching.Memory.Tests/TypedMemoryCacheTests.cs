using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Caching.Memory.Tests
{
    public class TypedMemoryCacheTests : TestBase
    {
        private const string Key = "test";
        private TypedMemoryCache<string, object>? _testSubject;

        public override void SetUp()
        {
            _testSubject = new TypedMemoryCache<string, object>(3);
        }

        [TestCase(TestName = "Memory cache calls factory only once")]
        public async Task GetOrCreateAsync_NormalFlow_CallsFactoryOnce()
        {
            // arrange
            var expected = await _testSubject!.GetOrCreateAsync(Key, () => Task.FromResult(new object()));
            bool factoryCalled = false;

            // act
            var result = await _testSubject.GetOrCreateAsync("test", () => { factoryCalled = true; return Task.FromResult(new object()); });

            // assert
            Assert.Multiple(() =>
            {
                Assert.False(factoryCalled);
                Assert.AreSame(expected, result);
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
                Assert.True(factoryCalled);
                Assert.AreNotSame(first, result);
            });
        }
    }
}
