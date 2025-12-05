using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

public class QuizController : Controller
{
    private readonly AppDbContext _db;
    public QuizController(AppDbContext db) { _db = db; }

    public async Task<IActionResult> List()
    {
        var quizzes = await _db.Quizzes.Include(q => q.Questions).ToListAsync();
        return View(quizzes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(int quizId, string studentNumber, string studentName)
    {
        if (string.IsNullOrWhiteSpace(studentNumber) || string.IsNullOrWhiteSpace(studentName))
        {
            TempData["Message"] = "Student ID and name are required.";
            return RedirectToAction("List");
        }

        var student = await _db.Students.FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);
        if (student == null)
        {
            student = new Student { StudentNumber = studentNumber, Name = studentName };
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
        }

        var existing = await _db.StudentAttempts.FirstOrDefaultAsync(a => a.StudentId == student.Id && a.QuizId == quizId);
        if (existing != null)
        {
            TempData["Message"] = "You have already taken this quiz. Retake is not allowed.";
            return RedirectToAction("List");
        }

        var attempt = new StudentAttempt { StudentId = student.Id, QuizId = quizId, StartTime = DateTime.UtcNow, Submitted = false };
        _db.StudentAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        return RedirectToAction("Take", new { attemptId = attempt.Id });
    }

    public async Task<IActionResult> Take(int attemptId)
    {
        var attempt = await _db.StudentAttempts
            .Include(a => a.Quiz).ThenInclude(q => q.Questions).ThenInclude(qt => qt.Choices)
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.Id == attemptId);

        if (attempt == null) return NotFound();

        return View(attempt);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int attemptId, IFormCollection form)
    {
        var attempt = await _db.StudentAttempts.Include(a => a.Quiz).FirstOrDefaultAsync(a => a.Id == attemptId);
        if (attempt == null) return NotFound();

        if (attempt.Submitted)
        {
            TempData["Message"] = "This quiz has already been submitted.";
            return RedirectToAction("List");
        }

        var elapsed = DateTime.UtcNow - attempt.StartTime;
        if (elapsed.TotalMinutes > attempt.Quiz.TimeLimitMinutes)
        {
            attempt.Submitted = true;
            attempt.EndTime = attempt.StartTime.AddMinutes(attempt.Quiz.TimeLimitMinutes);
            attempt.Score = 0;
            await _db.SaveChangesAsync();
            TempData["Message"] = "Time limit exceeded. Submission not accepted.";
            return RedirectToAction("List");
        }

        int totalCorrect = 0;
        var questions = await _db.Questions.Include(q => q.Choices).Where(q => q.QuizId == attempt.QuizId).ToListAsync();

        foreach (var q in questions)
        {
            var key = $"answers[{q.Id}]";
            var selected = form[key];
            int? selectedChoiceId = null;
            if (!string.IsNullOrEmpty(selected) && int.TryParse(selected, out int v)) selectedChoiceId = v;

            bool correct = q.Choices.Any(c => c.IsCorrect && c.Id == selectedChoiceId);
            if (correct) totalCorrect++;

            var ans = new StudentAnswer { AttemptId = attempt.Id, QuestionId = q.Id, SelectedChoiceId = selectedChoiceId };
            _db.StudentAnswers.Add(ans);
        }

        attempt.Submitted = true;
        attempt.EndTime = DateTime.UtcNow;
        attempt.Score = totalCorrect;
        await _db.SaveChangesAsync();

        TempData["Message"] = $"Quiz submitted. Your score: {totalCorrect} / {questions.Count}";
        return RedirectToAction("List");
    }

    public async Task<IActionResult> Report(int quizId)
    {
        var attempts = await _db.StudentAttempts.Where(a => a.QuizId == quizId && a.Submitted)
            .Include(a => a.Student)
            .ToListAsync();

        return View(attempts);
    }
}
