using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class TeacherController : Controller
{
    private readonly AppDbContext _db;
    public TeacherController(AppDbContext db) { _db = db; }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] QuizCreateDto dto)
    {
        if (dto == null || dto.Questions == null || dto.Questions.Count == 0)
            return BadRequest("Invalid quiz data");

        var quiz = new Quiz { Title = dto.Title, TimeLimitMinutes = dto.TimeLimitMinutes };
        _db.Quizzes.Add(quiz);
        await _db.SaveChangesAsync();

        foreach (var qDto in dto.Questions)
        {
            var q = new Question { QuizId = quiz.Id, Text = qDto.Text };
            _db.Questions.Add(q);
            await _db.SaveChangesAsync();

            foreach (var cDto in qDto.Choices)
            {
                var choice = new Choice { QuestionId = q.Id, Text = cDto.Text, IsCorrect = cDto.IsCorrect };
                _db.Choices.Add(choice);
            }
            await _db.SaveChangesAsync();
        }

        TempData["Message"] = "Quiz created successfully";
        return Ok(new { quizId = quiz.Id });
    }
}
