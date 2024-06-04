using AskForEtu.Core.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EmailSenderService.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions _options;
        private readonly SmtpClient _client;

        public EmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
            _client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_options.From, _options.Psw),
                Timeout = 25000
            };
        }
        public Task SendMailVerifyAsync(string toEmail, string resetLink)
        {
            return _client.SendMailAsync(
                new MailMessage(
                    from: _options.From,
                    to: toEmail,
                    "Email Verification Link",
                    $"You can use the following verification link to verify your account:" +
                    $"\n\nYour single-use verification Link: {resetLink}"));
        }

        public Task SendForgetPasswordAsync(string toEmail, string verifyCode)
        {
            var refCode = verifyCode.Split(';').Last();
            var resetCode = verifyCode.Split(';').First();

            return _client.SendMailAsync(
                new MailMessage(
                    from: _options.From,
                    to: toEmail,
                    "Password Reset Code",
                    $"Your request to reset your password has been confirmed. " +
                    $"Below is the verification code you need to use for the password reset process:" +
                    $"\n\nYour single-use verification code: [{resetCode}]" +
                    $"\n\nReferans Code: {refCode}"));
        }

        public Task SendAdminHasErrorAsync(string toEmail, string resetLink)
        {
            return _client.SendMailAsync(
                new MailMessage(
                    from: _options.From,
                    to: _options.From,
                    "Email Verification Link",
                    $"This mail should be sent to: {toEmail}.\n " +
                    $"You can use the following verification link to verify your account:" +
                    $"\n\nYour single-use verification Link: {resetLink}"));
        }
    }
}
