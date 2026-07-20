using Microsoft.AspNetCore.Mvc;
using Api.Application;
using Api.Application.Dtos;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ScoresController : ControllerBase
{
    private readonly IScoreQueryService _svc;
    public ScoresController(IScoreQueryService svc) { _svc = svc; }

    /// <summary>
    /// Lấy danh sách điểm chi tiết theo MSSV
    /// </summary>
    [HttpGet("detailed/{studentCode}")]
    public async Task<IActionResult> GetDetailedScores(string studentCode)
    {
        IReadOnlyList<DetailedStudyUnitScoreDto> list = await _svc.GetStudyUnitScoresDetailedAsync(studentCode);
        return Ok(list);
    }

    /// <summary>
    /// Gets component scores, optionally filtered by academic year and term.
    /// </summary>
    [HttpGet("components/{studentCode}")]
    public async Task<IActionResult> GetComponentScores(
        string studentCode,
        [FromQuery] string? yearStudy,
        [FromQuery] string? termId)
    {
        var list = await _svc.GetComponentScoresAsync(studentCode, yearStudy, termId);
        return Ok(list);
    }
}
