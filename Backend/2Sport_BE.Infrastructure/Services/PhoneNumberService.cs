using _2Sport_BE.Enums;
using _2Sport_BE.Repository.Interfaces;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace _2Sport_BE.Service.Services
{
    public interface IPhoneNumberService
    {
        Task<int> SendSmsToPhoneNumber(int userId, string phoneNumber);
    }
    public class PhoneNumberService : IPhoneNumberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PhoneNumberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> SendSmsToPhoneNumber(int userId, string phoneNumber)
        {
            try
            {

                var otp = GenerateOTP();
                var user = await _unitOfWork.UserRepository.FindAsync(userId);
                if (user == null)
                {
                    return (int)Errors.NotFoundUser;
                }
                if (!user.PhoneNumber.Equals(phoneNumber))
                {
                    user.PhoneNumber = phoneNumber;
                }

                user.OTP = otp;

                phoneNumber = ConvertToInternationalFormat(phoneNumber);

                var twilioAcount = (await _unitOfWork.TwilioRepository.GetAsync(_ => _.ToNumber.Equals(phoneNumber)))
                                                                      .FirstOrDefault();
                var accountSid = twilioAcount.AccountSId;
                var authToken = twilioAcount.AuthToken;
                TwilioClient.Init(accountSid, authToken);

                var to = new PhoneNumber(twilioAcount.ToNumber);
                var from = new PhoneNumber(twilioAcount.FromNumber);

                var message = MessageResource.Create(
                    to: to,
                    from: from,
                    body: $"Mã xác thực của bạn là: {otp}");

                await _unitOfWork.UserRepository.UpdateAsync(user);
                return 1;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (int)Errors.NotExcepted;
            }
            
        }

        private int GenerateOTP()
        {
            Random random = new Random();
            int otpNumber = random.Next(100000, 1000000); // Generates a number between 100000 and 999999
            return otpNumber;
        }

        private string ConvertToInternationalFormat(string phoneNumber)
        {
            if (phoneNumber.StartsWith("0"))
            {
                return "+84" + phoneNumber.Substring(1);
            }
            return phoneNumber; // Return the original if it doesn't start with 0
        }
    }
}
