using Microsoft.AspNetCore.Mvc;
using Api.Infrastructure.Persistence;
using Api.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
}
