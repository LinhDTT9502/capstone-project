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
using System.Text.Encodings.Web;


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
        Task<bool> SendRentalOrderInformationToCustomer(RentalOrder rentalOrder, List<RentalOrder> rentalOrders, string emailStr);
        Task<bool> SendRentalOrderReminder(RentalOrder rentalOrder, string email);
        bool IsValidEmail(string email);
        Task<bool> SenOTPMaillAsync(string email, string content);
    }

    public class MailService : IMailService
    {
        private const string VERIFY_EMAIL_CONSTANT = "Verify Your Mail";
        private const string FORGOT_PASSWORD_CONSTANT = "Reset Your Password";
        private const string ORDER_INFORMAION_CONSTANT = "SaleOrder Information";
        private const string RENTAL_ORDER_INFORMAION_CONSTANT = "RentalOrder Information";
        private const string RENTAL_ORDER_REMINDER_CONSTANT = "Information About Expired RentalOrder";
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
        private async Task<string> LoadEmailTemplateAsync(string templateFileName)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", templateFileName);
            return await File.ReadAllTextAsync(templatePath);
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
                var templateContent = await LoadEmailTemplateAsync("Forgot_Password_Email.html");

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
                var templateContent = await LoadEmailTemplateAsync("Verify_Email.html");

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
                var templateContent = await LoadEmailTemplateAsync("Order_Email.html");

                templateContent = templateContent
                    .Replace("{{OrderCode}}", saleOrder.OrderCode)
                    .Replace("{{CreatedAt}}", saleOrder.CreatedAt.ToString())
                    .Replace("{{TotalAmount}}", FormatCurrency(saleOrder.TotalAmount))
                    .Replace("{{FullName}}", saleOrder.FullName)
                    .Replace("{{Address}}", saleOrder.Address)
                    .Replace("{{ContactPhone}}", saleOrder.ContactPhone)
                    .Replace("{{Count}}", orderDetails.Count.ToString())
                    .Replace("{{SubTotal}}", FormatCurrency((decimal)saleOrder.SubTotal))
                    .Replace("{{TransportFee}}", saleOrder.TranSportFee.ToString());

                StringBuilder filledHtml = new StringBuilder();
                foreach (var item in orderDetails)
                {
                    string orderDetailHtml = $@"
                                <tr>
                                    <td width='60' valign='top' style='padding: 10px 0;'>
                                        <img src='{item.ImgAvatarPath}' alt='{item.ProductName}' style='width: 50px; height: auto;'>
                                    </td>
                                    <td valign='top' style='padding: 10px 5px;'>
                                        <p><strong>{item.ProductName}</strong></p>
                                        <p>Qty: {item.Quantity}</p>
                                    </td>
                                    <td align='right' style='padding: 10px; font-weight: bold;'>{FormatCurrency((decimal)item.UnitPrice)}</td>
                                </tr>";

                    filledHtml.Append(orderDetailHtml);
                }
                templateContent = templateContent.Replace("{{OrderDetails}}", filledHtml.ToString());
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
        public async Task<bool> SenOTPMaillAsync(string mail, string otp)
        {
            bool status = false;
            try
            {
                var templateContent = await LoadEmailTemplateAsync("Generate_OTP_Email.html");

                var emailContent = templateContent
                    .Replace("{{OTP}}", otp);

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
        public async Task<bool> SendRentalOrderInformationToCustomer(RentalOrder rentalOrder, List<RentalOrder>? rentalOrders, string emailStr)
        {
            bool status = false;
            try
            {
                var templateContent = await LoadEmailTemplateAsync("Rental_Order_Email.html");

                if (rentalOrders == null)
                {
                    rentalOrders = new List<RentalOrder> { rentalOrder };
                }
                templateContent = templateContent
                    .Replace("{{OrderCode}}", rentalOrder.ParentOrderCode)
                    .Replace("{{CreatedAt}}", rentalOrder.CreatedAt.ToString())
                    .Replace("{{TotalAmount}}", FormatCurrency(rentalOrder.TotalAmount))
                    .Replace("{{FullName}}", rentalOrder.FullName)
                    .Replace("{{Address}}", rentalOrder.Address)
                    .Replace("{{ContactPhone}}", rentalOrder.ContactPhone)
                    .Replace("{{Count}}", rentalOrders.Count.ToString())
                    .Replace("{{SubTotal}}", FormatCurrency((decimal)rentalOrder.SubTotal))
                    .Replace("{{TransportFee}}", rentalOrder.TranSportFee.ToString());

                string orderDetailHtml = "";

                orderDetailHtml = GenerateRentalDetailsHtml(rentalOrders);

                templateContent = templateContent.Replace("{{OrderDetails}}", orderDetailHtml.ToString());
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(emailStr));
                email.Subject = RENTAL_ORDER_INFORMAION_CONSTANT;
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
        private string GenerateRentalDetailsHtml(List<RentalOrder> rentalOrders)
        {
            StringBuilder filledHtml = new StringBuilder();

            foreach (var item in rentalOrders)
            {
                filledHtml.Append($@"
            <tr>
                <td width='60' valign='top' style='padding: 10px 0;'>
                    <img src='{HtmlEncoder.Default.Encode(item.ImgAvatarPath)}' alt='{HtmlEncoder.Default.Encode(item.ProductName)}' style='width: 50px; height: auto;'>
                </td>
                <td valign='top' style='padding: 10px 5px;'>
                    <p><strong>{HtmlEncoder.Default.Encode(item.ProductName)}</strong></p>
                    <p>Qty: {item.Quantity}</p>
                    <p><i>Ngày bắt đầu: {item.RentalStartDate:dd-MM-yyyy}</i></p>
                    <p><i>Ngày kết thúc: {item.RentalEndDate:dd-MM-yyyy}</i></p>
                </td>
                <td align='right' style='padding: 10px; font-weight: bold;'>{FormatCurrency((decimal)item.RentPrice)}</td>
            </tr>");
            }

            return filledHtml.ToString();
        }

        public async Task<bool> SendRentalOrderReminder(RentalOrder rentalOrder, string emailstr)
        {
            bool status = false;
            try
            {
                var templateContent = await LoadEmailTemplateAsync("Invoice_Reminder_Email.html");


                templateContent = templateContent
                    .Replace("{{OrderCode}}", rentalOrder.RentalOrderCode)
                    .Replace("{{RentalEndDate}}", rentalOrder.RentalEndDate.ToString())
                    .Replace("{{FullName}}", rentalOrder.FullName);

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(emailstr));
                email.Subject = RENTAL_ORDER_REMINDER_CONSTANT;
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
    }

}
