
using AskForEtu.Core.Behaviour;
using AskForEtu.Core.Dto.Mail;
using AskForEtu.Core.Services.Queue;
using EmailSenderService.EmailService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailSenderService
{
    public class SenderBgService : BackgroundService
    {
        private readonly ILogger<SenderBgService> _logger;
        private readonly ITaskQueue<EmailSendTemplate> _queue;
        private readonly IEmailSender _sender;
        public SenderBgService(ILogger<SenderBgService> logger,
            ITaskQueue<EmailSendTemplate> queue,
            IEmailSender sender)
        {
            _logger=logger;
            _queue=queue;
            _sender=sender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _queue.DeQueue(stoppingToken);

                try
                {
                    if (task.SendType == SendType.VerifyEmail)
                    {
                        await _sender.SendMailVerifyAsync(task.To, task.Content);
                        _logger.LogInformation($"[Email Verify Method] Email basarili bir sekilde gonderildi: {task.To}");
                    }
                    else
                    {
                        await _sender.SendForgetPasswordAsync(task.To, task.Content);
                        _logger.LogInformation($"[Password Reset Method] Email basarili bir sekilde gonderildi: {task.To}");
                    }
                }
                catch (Exception error)
                {
                    _logger.LogError($"[Error] Email gonderme islemi basarisiz: {task.To}");
                    _logger.LogError(error.Message);
                }
            }
        }
    }
}
