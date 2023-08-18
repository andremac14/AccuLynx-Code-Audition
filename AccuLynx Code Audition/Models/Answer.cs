using Newtonsoft.Json;

namespace AccuLynx_Code_Audition.Models
{
    public class Answer
    {
        [JsonProperty("question_id")]
        public int AnswerId { get; set; }

        [JsonProperty("body")]
        public string body { get; set; }

        [JsonProperty("is_accepted")]
        public bool IsAccepted { get; set; }
    }
}