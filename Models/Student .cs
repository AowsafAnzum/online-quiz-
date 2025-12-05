using System.ComponentModel.DataAnnotations;

public class Student
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string StudentNumber { get; set; } = null!;

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [EmailAddress]
    public string? Email { get; set; }
}
