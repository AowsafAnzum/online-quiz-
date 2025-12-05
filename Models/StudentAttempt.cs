using System;
using System.Collections.Generic;

public class StudentAttempt
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Score { get; set; }
    public bool Submitted { get; set; }
    public ICollection<StudentAnswer>? Answers { get; set; }
}
