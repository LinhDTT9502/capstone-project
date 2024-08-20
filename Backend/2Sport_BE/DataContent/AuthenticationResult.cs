using _2Sport_BE.Repository.Models;
using _2Sport_BE.ViewModels;
using Newtonsoft.Json;

namespace _2Sport_BE.DataContent
{
    public class TokenModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
        [JsonProperty("userId")]
        public int? UserId { get; set; }
    }

    public class AuthenticationResult : TokenModel
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
