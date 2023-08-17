using Newtonsoft.Json;

namespace AccuLynx_Code_Audition.Models
{
    public class StackOverflowResponse
    {
        [JsonProperty("items")]
        public List<Question> Items { get; set; }
        public bool has_more { get; set; }
        public int quota_max { get; set; }
        public int quota_remaining { get; set; }
    }
}