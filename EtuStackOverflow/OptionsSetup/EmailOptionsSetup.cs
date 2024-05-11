using AskForEtu.Core.Options;
using Microsoft.Extensions.Options;

namespace EtuStackOverflow.OptionsSetup
{
    public class EmailOptionsSetup : IConfigureOptions<EmailOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly string Section = "EmailOptions";

        public EmailOptionsSetup(IConfiguration configuration)
        {
            _configuration=configuration;
        }

        public void Configure(EmailOptions options)
        {
            _configuration.GetSection(Section).Bind(options);
        }
    }
}
