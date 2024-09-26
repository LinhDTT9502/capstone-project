using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using AutoMapper.Internal;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;


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
    public class SendEmailRequest
    {
        public string Email { get; set; }
    }
    public interface IMailService
    {
        Task<bool> SendVerifyEmailAsync(string verifyLink, string email);
        Task<bool> SendForgotPasswordEmailAsync(string resetLink, string email);
        Task<string> GenerateEmailVerificationTokenAsync(string email);
        bool IsValidEmail(string email);
    }

    public class MailService : IMailService
    {
        private const string VERIFY_EMAIL_CONSTANT = "Verify Your Mail";
        private const string FORGOT_PASSWORD_CONSTANT = "Reset Your Password";
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailService> _logger;
        private readonly MailSettings _mailSettings;
        private IUnitOfWork _unitOfWork;
        public MailService(
            IConfiguration configuration,
            ILogger<MailService> logger,
            IOptions<MailSettings> mailSettings,
            IUnitOfWork unitOfWork
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
            _unitOfWork = unitOfWork;
        }
        public bool IsValidEmail(string email)
        {
            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }
        public async Task<string> GenerateEmailVerificationTokenAsync(string email)
        {
            try
            {
                string secretKey = _configuration["ServiceConfiguration:JwtSettings:Secret"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenHandler = new JwtSecurityTokenHandler();

                var claims = new[]
                {
                    new Claim("Email", email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration["ServiceConfiguration:JwtSettings:TokenLifetime"])),
                    SigningCredentials = creds
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenStr = tokenHandler.WriteToken(token);

                return tokenStr;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> SendForgotPasswordEmailAsync(string resetLink, string mail)
        {
            bool status = false;
            try
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Forgot_Password_Email.html");
                var templateContent = await File.ReadAllTextAsync(templatePath);

                var emailContent = templateContent
                    .Replace("{{ChangePasswordLink}}", resetLink);

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(mail));
                email.Subject = FORGOT_PASSWORD_CONSTANT;
                email.Body = new TextPart("html") { Text = emailContent };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTlsWhenAvailable);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.SecrectKey);
                    await client.SendAsync(email);
                    await client.DisconnectAsync(true);
                }
                status = true;

            }
            catch (Exception ex)
            {
                status = false;
                _logger.LogError($"Error sending email: {ex.Message}");
                var errorLog = new ErrorLog
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                await _unitOfWork.ErrorLogRepository.InsertAsync(errorLog);
                _unitOfWork.Save();
            }
            return status;
        }
        public async Task<bool> SendVerifyEmailAsync(string verifyLink, string mail)
        {
            bool status = false;
            try
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Verify_Email.html");
                var templateContent = await File.ReadAllTextAsync(templatePath);

                var emailContent = templateContent
                    .Replace("{{VerifyLink}}", verifyLink);

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(mail));
                email.Subject = VERIFY_EMAIL_CONSTANT;
                email.Body = new TextPart("html") { Text = emailContent };
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTlsWhenAvailable);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.SecrectKey);
                    await client.SendAsync(email);
                    await client.DisconnectAsync(true);
                }
                status = true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                status = false;
                var errorLog = new ErrorLog
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                await _unitOfWork.ErrorLogRepository.InsertAsync(errorLog);
                _unitOfWork.Save();
            }
            return status;
        }
    }
    
}
