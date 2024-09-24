namespace social_oc_api.Models.Domain.Auth
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public Boolean IsRevoked { get; set; }
    }
}
