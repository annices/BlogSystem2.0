
namespace BlogSystem.Services
{
    public interface IEmailSender
    {
        public void SendEmail(string to, string from, string subject, string message);
    }
}
