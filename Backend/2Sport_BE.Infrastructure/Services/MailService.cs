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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


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
        Task<bool> SendSaleOrderInformationToCustomer(SaleOrder saleOrder, List<OrderDetail> orderDetails, string emailStr);
        bool IsValidEmail(string email);
        Task<bool> SendEMailAsync(string email, string content);
    }

    public class MailService : IMailService
    {
        private const string VERIFY_EMAIL_CONSTANT = "Verify Your Mail";
        private const string FORGOT_PASSWORD_CONSTANT = "Reset Your Password";
        private const string ORDER_INFORMAION_CONSTANT = "SaleOrder Information";

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
                var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Forgot_Password_Email.html");
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
                await _unitOfWork.SaveChanges();
            }
            return status;
        }
        public async Task<bool> SendVerifyEmailAsync(string verifyLink, string mail)
        {
            bool status = false;
            try
            {
                var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Verify_Email.html");
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
                await _unitOfWork.SaveChanges();
            }
            return status;
        }
        public string FormatCurrency(decimal amount)
        {
            return string.Format("{0:N0} VND", amount);
        }
        public async Task<bool> SendSaleOrderInformationToCustomer(SaleOrder saleOrder, List<OrderDetail> orderDetails, string emailStr)
        {
            bool status = false;
            try
            {
                var templatePath = "C:\\Users\\NguyenTuanVu\\Desktop\\Capstone\\new_brand\\capstone-project\\Backend\\2Sport_BE\\Templates\\Order_Email.html";
                var templateContent = await File.ReadAllTextAsync(templatePath);

                templateContent = templateContent
                    .Replace("{{OrderCode}}", saleOrder.OrderCode);
                templateContent = templateContent
                    .Replace("{{CreatedAt}}", saleOrder.CreatedAt.ToString());
                templateContent = templateContent
                    .Replace("{{TotalAmount}}", FormatCurrency(saleOrder.TotalAmount));
                templateContent = templateContent
                    .Replace("{{FullName}}", saleOrder.FullName);
                templateContent = templateContent
                    .Replace("{{Address}}", saleOrder.Address);
                templateContent = templateContent
                    .Replace("{{ContactPhone}}", saleOrder.ContactPhone);
                templateContent = templateContent
                   .Replace("{{Count}}", orderDetails.Count.ToString());
                templateContent = templateContent
                   .Replace("{{SubTotal}}", FormatCurrency((decimal)saleOrder.SubTotal));
                templateContent = templateContent
                    .Replace("{{TransportFee}}", saleOrder.TranSportFee.ToString());
                StringBuilder filledHtml = new StringBuilder();
                foreach (var item in orderDetails)
                {
                    string productHtml = templateContent;

                    // Replace the placeholders with actual product data
                    templateContent = templateContent.Replace("{{ProductImage}}", item.ImgAvatarPath)
                                             .Replace("{{ProductName}}", item.ProductName)
                                             .Replace("{{Quantity}}", item.Quantity.ToString())
                                             .Replace("{{UnitPrice}}", FormatCurrency((decimal)item.UnitPrice));

                    // Append this product's HTML to the final HTML
                    filledHtml.Append(templateContent);
                }
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(emailStr));
                email.Subject = ORDER_INFORMAION_CONSTANT;
                email.Body = new TextPart("html") { Text = templateContent };

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
                await _unitOfWork.SaveChanges();
            }
            return status;
        }

        public async Task<bool> SendEMailAsync(string email, string content)
        {
            bool status = false;
            /*try
            {
                var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Forgot_Password_Email.html");
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
                await _unitOfWork.SaveChanges();
            }*/
            return status;
        }
    }

}
