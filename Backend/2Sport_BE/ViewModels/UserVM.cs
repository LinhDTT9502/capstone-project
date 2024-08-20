using _2Sport_BE.Repository.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.ViewModels
{
    public class UserDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
    }
    public class UserVM
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? RoleName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }

    }
    public class UserCM : UserDTO
    {
        public string? Email { get; set; }

    }
    public class UserUM : UserDTO
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Salary { get; set; }
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
        [JsonProperty("oldPassword")]
        public string OldPassword { get; set; }
        [Required]
        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
}
