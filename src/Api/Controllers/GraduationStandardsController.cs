using Api.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/students/graduation-standards")]
public class GraduationStandardsController : ControllerBase
{
    private readonly GraduationStandardQueryService _service;

    public GraduationStandardsController(GraduationStandardQueryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets the configured graduation standards and consolidated achievement evidence for a student.
    /// </summary>
    [HttpGet("{studentCode}")]
    public async Task<IActionResult> Get(
        string studentCode,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAsync(studentCode, cancellationToken));
    }
}
