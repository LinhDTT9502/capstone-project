using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{
    public class UserDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
    public class UserVM
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }

    }
    public class UserCM : UserDTO
    {
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? RoleId { get; set; }
    }
    public class RegisterModel : UserDTO
    {
    }
    public class UserUM
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }    
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
        public string? UserName { get; set; }
        [JsonProperty("password")]
        public string? Password { get; set; }
    }
    public class ForgotVM
    {
        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
    public class ChangePasswordVM
    {
        [Required]
        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
