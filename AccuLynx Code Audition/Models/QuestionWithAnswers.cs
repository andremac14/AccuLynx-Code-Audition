namespace AccuLynx_Code_Audition.Models
{
    public class QuestionWithAnswers
    {
        public int QuestionId { get; set; }
        public string Title { get; set; }
        public List<Answer> Answers { get; set; }
    }
}