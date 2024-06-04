
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
                    _logger.LogError($"[SendMailError] Email gonderme islemi basarisiz: {task.To}");
                    _logger.LogError($"[SendMailErrorDetail] Mail:{task.To}" +
                                    $"\n\t[Description] {error.Message}" +
                                    (error.InnerException != null ? $"\n\t{error.InnerException}" : ""));
                    if (task.SendType == SendType.VerifyEmail)
                    {
                        if (task.RetryCount > 0)
                        {
                            _logger.LogInformation($"[SendMailError-Retry] Email gonderme islemi tekrarlandı: {task.To}");
                            task.RetryCount--;
                            await _queue.AddQueue(task);
                        }
                        else
                        {
                            try
                            {
                                await _sender.SendAdminHasErrorAsync(task.To, error.Message);
                            }
                            catch (Exception inner)
                            {
                                _logger.LogError("[SendMailToAdminError] Admin Mail Gonderme Islemi Basarisiz");
                                _logger.LogError(inner.Message);
                                continue;
                            }
                        }
                    }

                }
            }
        }
    }
}
