using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace _2Sport_BE.Services
{
    public class MailSettings
    {
        public string SecrectKey { get; set; }
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
    }
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }

    public interface ISendMailService
    {
        Task<bool> SendEmailAsync(MailRequest mailRequest);

    }

    public class MailService : ISendMailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailService> _logger;
        private readonly MailSettings _mailSettings;

        public MailService(
            IConfiguration configuration,
            ILogger<MailService> logger,
            IOptions<MailSettings> mailSettings
            )
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _mailSettings = new MailSettings
            {
                SecrectKey = _configuration["AppSettings:SecrectKey"],
                Mail = _configuration["AppSettings:MailSettings:Mail"],
                DisplayName = _configuration["AppSettings:MailSettings:DisplayName"],
                Host = _configuration["AppSettings:MailSettings:Host"],
                Port = _configuration.GetValue<int>("AppSettings:MailSettings:Port"),
                EnableSSL = _configuration.GetValue<bool>("AppSettings:MailSettings:EnableSSL")
            };
        }
        public async Task<bool> SendEmailAsync(MailRequest mailRequest)
        {
            bool status = false;
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();
                if (mailRequest.Attachments != null)
                {
                    byte[] fileBytes;
                    foreach (var file in mailRequest.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                fileBytes = ms.ToArray();
                            }
                            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                        }
                    }
                }
                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTlsWhenAvailable);
                    client.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                    client.Authenticate(_mailSettings.Mail, _mailSettings.SecrectKey);
                    client.Send(email);
                    client.Disconnect(true);
                }
                status = true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                status = false;
            }
            return status;
        }
    }
    
}
