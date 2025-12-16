using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application;
using Api.Application.Dtos;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

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
}
