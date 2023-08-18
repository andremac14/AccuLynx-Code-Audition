using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace AccuLynx_Code_Audition.Models
{
    public class Question
    {
        [JsonProperty("question_id")]
        public int QuestionId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("is_answered")]
        public bool IsAnswered { get; set; }

        [JsonProperty("answer_count")]
        public int AnswerCount { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("body")]
        public string body { get; set; }

        [JsonProperty("score")]
        public string score { get; set; }

        public List<Answer> Answers { get; set; }

        [JsonProperty("accepted_answer_id")]
        public int AnswerId { get; set; }
    }
}