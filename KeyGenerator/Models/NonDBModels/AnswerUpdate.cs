namespace KeyGenerator.Models.NonDBModels
{
    public class AnswerUpdate
    {
        public int PaperID { get; set; }
        public int QuestionNumber { get; set; }
        public string Answer { get; set; }
        public string PreviousAnswer { get; set; }
    }
}
