public class Choice
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
    public string Text { get; set; } = null!;
    public bool IsCorrect { get; set; }
}

