namespace social_oc_api.Models.DTO.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
        public string? NameUser { get; set; }
    }
}
