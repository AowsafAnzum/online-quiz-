using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class Quiz
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Range(1, 180)]
    public int TimeLimitMinutes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Question>? Questions { get; set; }
}
