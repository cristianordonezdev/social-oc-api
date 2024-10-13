namespace social_oc_api.Models.Domain
{
    public class ProfileUser
    {
        public ApplicationUser User { get; set; }

        public MetricsProfile MetricsProfile { get; set; }
    }
}
