using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Infrastructure.DTOs
{
    public class UserDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        public string Address { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
    public class UserCM : UserDTO
    {
        [Required]
        public string? HashPassword { get; set; }
    }
    public class UserVM : UserDTO
    {
        public int Id { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string? ImgAvatarPath { get; set; }
        public bool? IsActived { get; set; }
        public CustomerVM? CustomerDetail { get; set; }
        public StaffVM? StaffDetail { get; set; }
        public ManagerVM? ManagerDetail { get; set; }
    }
    public class UserUM : UserDTO
    {

    }
    public class RegisterModel
    {

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
    public class ProfileUM
    {
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }
    }
    public class UserLogin
    {
        [JsonProperty("userName")]
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class ForgotVM
    {
        [JsonProperty("userName")]
        [Required(ErrorMessage = "Email is required")]
        public string Username { get; set; }

        [JsonProperty("email")]
        [Required(ErrorMessage = "Email is required")]
        /*[EmailAddress(ErrorMessage = "Email Address is invalid format")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]*/
        public string Email { get; set; }
    }
    public class ChangePasswordVM
    {
        [JsonProperty("oldPassword")]
        [Required(ErrorMessage = "oldPassword is required")]
        public string OldPassword { get; set; }
        [JsonProperty("newPassword")]
        [Required(ErrorMessage = "NewPassword is required")]
        public string NewPassword { get; set; }
    }
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "ResetPasswordToken is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "NewPassword is required")]

        public string NewPassword { get; set; }
    }
    public class VerifyOTPMobile
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "OtpCode is required")]
        public string OtpCode { get; set; }
    }
    public class ResetPasswordMobile
    {
        [Required(ErrorMessage = "OtpCode is required")]
        public string OtpCode { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "NewPassword is required")]
        public string NewPassword { get; set; }
    }

    public class ResetEmailRequesrt
    {

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OtpCode is required")]
        public string OtpCode { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

    }
    public class AvatarModel
    {
        public int userId { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
