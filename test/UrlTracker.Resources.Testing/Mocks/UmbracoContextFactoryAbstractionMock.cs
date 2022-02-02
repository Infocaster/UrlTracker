using Moq;
using UrlTracker.Core.Abstractions;

namespace UrlTracker.Resources.Testing.Mocks
{
    public class UmbracoContextFactoryAbstractionMock
    {
        private readonly Mock<IUmbracoContextFactoryAbstraction> _abstractionMock;
        private readonly Mock<IUmbracoContextReferenceAbstraction> _crefAbstractionMock;

        public UmbracoContextFactoryAbstractionMock()
        {
            _abstractionMock = new Mock<IUmbracoContextFactoryAbstraction>();
            _crefAbstractionMock = new Mock<IUmbracoContextReferenceAbstraction>();

            _abstractionMock.Setup(obj => obj.EnsureUmbracoContext()).Returns(_crefAbstractionMock.Object);
        }

        public IUmbracoContextFactoryAbstraction UmbracoContextFactory => _abstractionMock.Object;

        public Mock<IUmbracoContextReferenceAbstraction> CrefMock => _crefAbstractionMock;
        public Mock<IUmbracoContextFactoryAbstraction> UmbracoContextFactoryMock => _abstractionMock;
    }
}
