using AskForEtu.Core.Dto.Mail;
using AskForEtu.Core.Services.Queue;
using Microsoft.Extensions.Configuration;
using System.Threading.Channels;

namespace AskForEtu.Repository.Services.Queue
{
    public class EmailTaskQueue : ITaskQueue<EmailSendTemplate>
    {
        public readonly Channel<EmailSendTemplate> queue;
        
        public EmailTaskQueue(IConfiguration configuration)
        {
            int.TryParse(configuration["QueueCapacity"], out int capacity);

            BoundedChannelOptions options = new(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            queue = Channel.CreateBounded<EmailSendTemplate>(options);
        }

        public async Task AddQueue(EmailSendTemplate task)
        {
            await queue.Writer.WriteAsync(task);
        }

        public async Task<EmailSendTemplate> DeQueue(CancellationToken cancellationToken)
        {
            var task = await queue.Reader.ReadAsync(cancellationToken);

            return task;
        }
    }
}
