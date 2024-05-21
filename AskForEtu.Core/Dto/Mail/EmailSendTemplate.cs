using AskForEtu.Core.Behaviour;

namespace AskForEtu.Core.Dto.Mail
{
    public class EmailSendTemplate
    {
        public string To { get; init; }
        public string Content { get; init; }
        public SendType SendType { get; init; }
    }
}
