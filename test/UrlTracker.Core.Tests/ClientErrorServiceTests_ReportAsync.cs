using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Tests
{
    public partial class ClientErrorServiceTests
    {
        [TestCase(TestName = "ReportAsync creates new client error if none exist")]
        public async Task ReportAsync_NoClientErrorExists_CreatesNew()
        {
            // arrange
            const string inputUrl = "https://urltracker.ic/";
            ClientErrorRepositoryMock.Setup(obj => obj.GetAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<int?>(), It.IsAny<string?>()))
                                     .ReturnsAsync(Array.Empty<IClientError>());
            ClientErrorRepositoryMock.Setup(obj => obj.Save(It.Is<IClientError>(e => e.Url == inputUrl)))
                                     .Verifiable();

            // act
            await _testSubject.ReportAsync(inputUrl, new DateTime(2020, 7, 23), null);

            // assert
            ClientErrorRepositoryMock.Verify();
        }

        [TestCase(TestName = "ReportAsync aborts registration if client error is ignored")]
        public async Task ReportAsync_ClientErrorIgnored_DoesNotReport()
        {
            // arrange
            const string inputUrl = "https://urltracker.ic/";
            ClientErrorRepositoryMock.Setup(obj => obj.GetAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<int?>(), It.IsAny<string?>()))
                                     .ReturnsAsync(new[] { new ClientErrorEntity(inputUrl, true, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound) });

            // act
            await _testSubject.ReportAsync(inputUrl, new DateTime(2020, 7, 23), null);

            // assert
            ClientErrorRepositoryMock.Verify(obj => obj.Report(It.IsAny<IClientError>(), It.IsAny<DateTime>(), It.IsAny<IReferrer?>()), Times.Never);
        }

        [TestCase(TestName = "ReportAsync creates a new referrer if none exist")]
        public async Task ReportAsync_ReferrerDoesNotExist_NewReferrerCreated()
        {
            // arrange
            const string inputUrl = "https://urltracker.ic/";
            const string inputReferrer = "https://urltracker.ic/lorem";
            ClientErrorRepositoryMock.Setup(obj => obj.GetAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<int?>(), It.IsAny<string?>()))
                                     .ReturnsAsync(new[] { new ClientErrorEntity(inputUrl, false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound) });
            ReferrerRepositoryMock.Setup(obj => obj.Get(inputReferrer)).Returns((string _) => null);
            ReferrerRepositoryMock.Setup(obj => obj.Save(It.IsAny<IReferrer>())).Verifiable();

            // act
            await _testSubject.ReportAsync(inputUrl, new DateTime(2020, 7, 23), inputReferrer);

            // assert
            ReferrerRepositoryMock.Verify();
        }

        [TestCase(TestName = "ReportAsync takes existing models if possible")]
        public async Task ReportAsync_ModelsExist_UseExistingModels()
        {
            // arrange
            const string inputUrl = "https://urltracker.ic/";
            const string inputReferrer = "https://urltracker.ic/lorem";
            ClientErrorRepositoryMock.Setup(obj => obj.GetAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<int?>(), It.IsAny<string?>()))
                                     .ReturnsAsync(new[] { new ClientErrorEntity(inputUrl, false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound) });
            ReferrerRepositoryMock.Setup(obj => obj.Get(inputReferrer))
                                  .Returns(new ReferrerEntity(inputReferrer));

            // act
            await _testSubject.ReportAsync(inputUrl, new DateTime(2020, 7, 23), inputReferrer);

            // assert
            ClientErrorRepositoryMock.Verify(obj => obj.Save(It.IsAny<IClientError>()), Times.Never);
            ReferrerRepositoryMock.Verify(obj => obj.Save(It.IsAny<IReferrer>()), Times.Never);
            ClientErrorRepositoryMock.Verify(obj => obj.Report(It.IsAny<IClientError>(), It.IsAny<DateTime>(), It.IsAny<IReferrer?>()), Times.Once);
        }
    }
}
