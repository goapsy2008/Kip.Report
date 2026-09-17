using System.Threading.Channels;

namespace Kip.Report.Application;

public class BackgroundReportQueue
{
    private readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>();

    public async ValueTask QueueReportAsync(Guid queryId, CancellationToken cancellationToken = default)
    {
        await _queue.Writer.WriteAsync(queryId, cancellationToken);
    }

    public async ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
