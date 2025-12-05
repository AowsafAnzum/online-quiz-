using System.Collections.Generic;

public class QuizCreateDto
{
    public string Title { get; set; } = null!;
    public int TimeLimitMinutes { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}
public class QuestionDto
{
    public string Text { get; set; } = null!;
    public List<ChoiceDto> Choices { get; set; } = new();
}
public class ChoiceDto
{
    public string Text { get; set; } = null!;
    public bool IsCorrect { get; set; }
}
