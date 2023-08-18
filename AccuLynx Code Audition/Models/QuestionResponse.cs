using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace AccuLynx_Code_Audition.Models
{
    public class QuestionResponse
    {
        public List<Question> Items { get; set; }
    }
}