using Examine.Lucene;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Infrastructure.Examine;

namespace UrlTracker.IntegrationTests.Utils;

internal sealed class ExamineInMemoryConfiguration : IConfigureNamedOptions<LuceneDirectoryIndexOptions>
{
    public void Configure(string? name, LuceneDirectoryIndexOptions options)
    {
        // Configure lucene directory to be in memory so that we don't rely on the filesystem while running tests
        options.DirectoryFactory = new LuceneRAMDirectoryFactory();
    }

    public void Configure(LuceneDirectoryIndexOptions options)
        => throw new NotImplementedException("This is never called and is just part of the interface");
}
