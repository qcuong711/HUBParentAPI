using Microsoft.AspNetCore.Mvc;
using Api.Infrastructure.Persistence;
using Api.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AveragesController : ControllerBase
{
    private readonly AppDbContext _db;
    public AveragesController(AppDbContext db) { _db = db; }

    /// <summary>
    /// Lấy danh sách điểm trung bình theo năm học
    /// </summary>
    [HttpGet("year/{studentCode}")]
    public async Task<IActionResult> GetByYear(string studentCode)
    {
        var list = await _db.StudentAverageScoresByYearStudy.AsNoTracking()
            .Where(x => x.StudentID == studentCode)
            .OrderBy(x => x.YearStudy)
            .Select(x => new YearStudyAverageDto
            {
                YearStudy = x.YearStudy,
                AverageScore10 = x.AverageScore10,
                AverageScore4 = x.AverageScore4,
                AverageGatherScore10 = x.AverageGatherScore10,
                AverageGatherScore4 = x.AverageGatherScore4,
                UpdateDate = x.UpdateDate
            })
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>
    /// Lấy danh sách điểm trung bình tích lũy theo học kỳ
    /// </summary>
    [HttpGet("terms/{studentCode}")]
    public async Task<IActionResult> GetByTerms(string studentCode)
    {
        var list = await _db.StudentAverageGatherScoresByTerms.AsNoTracking()
            .Where(x => x.StudentID == studentCode)
            .OrderBy(x => x.YearStudy).ThenBy(x => x.OrderTerm)
            .Select(x => new TermGatherAverageDto
            {
                YearStudy = x.YearStudy,
                TermID = x.TermID,
                OrderTerm = x.OrderTerm,
                AverageScore10 = x.AverageScore10,
                AverageScore4 = x.AverageScore4,
                AverageGatherScore10 = x.AverageGatherScore10,
                AverageGatherScore4 = x.AverageGatherScore4,
                UpdateDate = x.UpdateDate
            })
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>
    /// Lấy tổng hợp điểm trung bình chung
    /// </summary>
    [HttpGet("overall/{studentCode}")]
    public async Task<IActionResult> GetOverall(string studentCode)
    {
        var x = await _db.StudentAverageScores.AsNoTracking()
            .Where(y => y.StudentID == studentCode)
            .OrderByDescending(y => y.UpdateDate)
            .Select(y => new OverallAverageDto
            {
                AverageScore10 = y.AverageScore10,
                AverageScore4 = y.AverageScore4,
                AverageGatherScore10 = y.AverageGatherScore10,
                AverageGatherScore4 = y.AverageGatherScore4,
                IsModified = y.IsModified,
                UpdateStaff = y.UpdateStaff,
                UpdateDate = y.UpdateDate
            })
            .FirstOrDefaultAsync();
        return Ok(x);
    }

    /// <summary>
    /// Gets graduation progress for each study program of the student.
    /// </summary>
    [HttpGet("graduation/{studentCode}")]
    public async Task<IActionResult> GetGraduationProgress(string studentCode)
    {
        var rows = await (
            from graduation in _db.StudentAverageScoresGraduation.AsNoTracking()
            where graduation.StudentID == studentCode
            join program in _db.StudyPrograms.AsNoTracking()
                on graduation.StudyProgramID equals program.StudyProgramID into programJoin
            from program in programJoin.DefaultIfEmpty()
            select new
            {
                graduation.StudentID,
                graduation.StudyProgramID,
                program.StudyProgramName,
                graduation.MandatoryCredits,
                graduation.MandatoryGatherCredits,
                graduation.SelectiveCredits,
                graduation.SelectiveGatherCredits,
                graduation.NumberOfCurriculumFails,
                graduation.NumberOfCreditsFails,
                AverageGatherScore10 = graduation.AverageGatherScore,
                graduation.AverageGatherScore4,
                graduation.UpdateDate
            })
            .ToListAsync();

        var list = rows.Select(x =>
        {
            var mandatoryCredits = x.MandatoryCredits ?? 0;
            var selectiveCredits = x.SelectiveCredits ?? 0;
            var mandatoryGatherCredits = x.MandatoryGatherCredits ?? 0;
            var selectiveGatherCredits = x.SelectiveGatherCredits ?? 0;
            var requiredCredits = mandatoryCredits + selectiveCredits;
            var completedCredits = mandatoryGatherCredits + selectiveGatherCredits;
            var remainingCredits = Math.Max(requiredCredits - completedCredits, 0);
            decimal? progressPercent = requiredCredits > 0
                ? Math.Min(Math.Round(completedCredits / requiredCredits * 100, 2), 100)
                : null;

            return new GraduationProgressDto
            {
                StudentID = x.StudentID!,
                StudyProgramID = x.StudyProgramID,
                StudyProgramName = x.StudyProgramName,
                MandatoryCredits = x.MandatoryCredits,
                MandatoryGatherCredits = x.MandatoryGatherCredits,
                SelectiveCredits = x.SelectiveCredits,
                SelectiveGatherCredits = x.SelectiveGatherCredits,
                RequiredCredits = requiredCredits,
                CompletedCredits = completedCredits,
                RemainingCredits = remainingCredits,
                ProgressPercent = progressPercent,
                NumberOfCurriculumFails = x.NumberOfCurriculumFails,
                NumberOfCreditsFails = x.NumberOfCreditsFails,
                AverageGatherScore10 = x.AverageGatherScore10,
                AverageGatherScore4 = x.AverageGatherScore4,
                UpdateDate = x.UpdateDate
            };
        }).ToList();

        return Ok(list);
    }
}
