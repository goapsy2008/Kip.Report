using Kip.Report.Domain.Models;
using Kip.Report.Domain.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Kip.Report.Application.Tests
{
    public class QueryServiceTests
    {
        [Fact]
        public async Task CreateQueryAndGetInfoBeforeTimeoutExceedAndAfterExceed()
        {
            // Arrange 
            var fakeTimeProvider = new FakeTimeProvider();

            var query = new Query { UserId = Guid.NewGuid(), From = DateTimeOffset.UtcNow.AddHours(-1), To = DateTimeOffset.UtcNow };
            Query? repoQuery = null;

            var fakeRepository = new Mock<IQueryRepository>();
            fakeRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Query>(), It.IsAny<CancellationToken>()))
                .Callback((Query query, CancellationToken ct) => repoQuery = query)
                .ReturnsAsync((Query query, CancellationToken ct) => query.Id);
            fakeRepository
                .Setup(repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken ct) => repoQuery);

            var options = Options.Create(new QueryOptions { Timeout = 60000 });

            var service = new QueryService(fakeRepository.Object, options, fakeTimeProvider);

            // Act
            var queryId = await service.AddAsync(query, CancellationToken.None);
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(options.Value.Timeout / 2));
            var queryBeforeExceed = await service.GetAsync(queryId, CancellationToken.None);

            // Assert
            Assert.Equal(50, queryBeforeExceed.Percent);
            Assert.Null(queryBeforeExceed.Report);

            // Act
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(options.Value.Timeout / 2));
            var queryAfterExceed = await service.GetAsync(queryId, CancellationToken.None);

            // Assert
            Assert.Equal(100, queryAfterExceed.Percent);
            Assert.NotNull(queryAfterExceed.Report);
            Assert.Equal(12, queryAfterExceed.Report.CountSignIn);
        }
    }
}
