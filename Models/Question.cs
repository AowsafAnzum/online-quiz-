using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class Question
{
    public int Id { get; set; }
    [Required]
    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }
    [Required]
    public string Text { get; set; } = null!;
    public ICollection<Choice>? Choices { get; set; }
}
