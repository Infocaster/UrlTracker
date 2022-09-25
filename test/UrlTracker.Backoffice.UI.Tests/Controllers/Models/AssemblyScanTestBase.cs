using System.Reflection;
using NUnit.Framework;

namespace UrlTracker.Backoffice.UI.Tests.Controllers.Models
{
    public class AssemblyScanTestBase<T>
    {
        protected static IEnumerable<T> GetInstancesOfEachType(params Assembly[] assemblies)
        {
            if (assemblies?.Any() != true) assemblies = new Assembly[] { typeof(T).Assembly };

            return from assembly in assemblies
                   from type in assembly.GetTypes()
                   where !(type.IsAbstract || type.IsInterface || type.IsGenericType) && typeof(T).IsAssignableFrom(type)
                   let ctor = type.GetConstructor(Type.EmptyTypes)
                   where ctor is not null
                   select (T)ctor.Invoke(Array.Empty<object>());
        }

        protected static IEnumerable<TestCaseData> CreateTestCaseForEveryType(Func<T, TestCaseData> factory)
        {
            return GetInstancesOfEachType().Select(factory);
        }
    }
}
