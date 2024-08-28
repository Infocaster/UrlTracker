using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace UrlTracker.Backoffice.Notifications.Cleanup
{
    internal class CleanupQueue
    {
        private Channel<object> _channel;

        public CleanupQueue()
        {
            var channelOptions = new BoundedChannelOptions(1)
            {
                FullMode = BoundedChannelFullMode.DropWrite,
                SingleReader = true,
                SingleWriter = true,
            };
            _channel = Channel.CreateBounded<object>(channelOptions);
        }

        public ValueTask ScheduleAsync()
        {
            return _channel.Writer.WriteAsync(new object());
        }

        public ValueTask<object> WaitForSignalAsync(CancellationToken stoppingToken)
        {
            return _channel.Reader.ReadAsync(stoppingToken);
        }
    }
}
