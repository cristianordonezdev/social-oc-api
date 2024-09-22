namespace social_oc_api.Models.Errors
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
    }
}
