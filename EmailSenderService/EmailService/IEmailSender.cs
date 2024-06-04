namespace EmailSenderService.EmailService
{
    public interface IEmailSender
    {
        Task SendMailVerifyAsync(string toEmail, string resetLink);
        Task SendForgetPasswordAsync(string toEmail, string verifyCode);
        Task SendAdminHasErrorAsync(string toEmail, string resetLink);
    }
}
