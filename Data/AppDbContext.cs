using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{




   
    protected AppDbContext()
    {
    }





    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Quiz> Quizzes { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<Choice> Choices { get; set; } = null!;
    public DbSet<StudentAttempt> StudentAttempts { get; set; } = null!;
    public DbSet<StudentAnswer> StudentAnswers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // prevent multiple attempts by unique index
        modelBuilder.Entity<StudentAttempt>()
            .HasIndex(a => new { a.StudentId, a.QuizId })
            .IsUnique();

        modelBuilder.Entity<Question>()
            .HasMany(q => q.Choices)
            .WithOne(c => c.Question)
            .HasForeignKey(c => c.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
