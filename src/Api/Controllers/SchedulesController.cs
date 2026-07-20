using Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleQueryService _service;

    public SchedulesController(IScheduleQueryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets the timetable; defaults to the current term.
    /// </summary>
    [HttpGet("timetable/{studentCode}")]
    public async Task<IActionResult> GetTimetable(
        string studentCode,
        [FromQuery] string? yearStudy,
        [FromQuery] string? termId)
    {
        return Ok(await _service.GetTimetableAsync(studentCode, yearStudy, termId));
    }

    /// <summary>
    /// Gets the exam or project schedule; defaults to the current term.
    /// </summary>
    [HttpGet("exams/{studentCode}")]
    public async Task<IActionResult> GetExams(
        string studentCode,
        [FromQuery] string? yearStudy,
        [FromQuery] string? termId)
    {
        return Ok(await _service.GetExamsAsync(studentCode, yearStudy, termId));
    }
}
