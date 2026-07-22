using Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentQueryService _service;

    public StudentsController(IStudentQueryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets registered courses and the student's total study load.
    /// </summary>
    [HttpGet("registrations/{studentCode}")]
    public async Task<IActionResult> GetRegistrations(
        string studentCode,
        [FromQuery] string? yearStudy,
        [FromQuery] string? termId)
    {
        return Ok(await _service.GetRegistrationsAsync(studentCode, yearStudy, termId));
    }

    /// <summary>
    /// Gets the latest study status and academic warning history.
    /// </summary>
    [HttpGet("study-status/{studentCode}")]
    public async Task<IActionResult> GetStudyStatus(
        string studentCode,
        [FromQuery] string? yearStudy,
        [FromQuery] string? termId)
    {
        return Ok(await _service.GetStudyStatusesAsync(studentCode, yearStudy, termId));
    }

    /// <summary>
    /// Gets the student's study programs, majors, and specializations.
    /// </summary>
    [HttpGet("programs/{studentCode}")]
    public async Task<IActionResult> GetPrograms(string studentCode)
    {
        return Ok(await _service.GetProgramsAsync(studentCode));
    }

    /// <summary>
    /// Gets academic advisor contacts for the student's class.
    /// </summary>
    [HttpGet("advisor/{studentCode}")]
    public async Task<IActionResult> GetAdvisor(
        string studentCode,
        [FromQuery] string? yearStudy,
        [FromQuery] string? termId)
    {
        return Ok(await _service.GetAdvisorsAsync(studentCode, yearStudy, termId));
    }
}
