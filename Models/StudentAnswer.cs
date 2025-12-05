public class StudentAnswer
{
    public int Id { get; set; }
    public int AttemptId { get; set; }
    public StudentAttempt? Attempt { get; set; }
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
    public int? SelectedChoiceId { get; set; }
    public Choice? SelectedChoice { get; set; }
}
