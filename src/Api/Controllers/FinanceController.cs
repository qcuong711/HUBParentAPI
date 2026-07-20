using Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/finance")]
public class FinanceController : ControllerBase
{
    private readonly IFinanceQueryService _service;

    public FinanceController(IFinanceQueryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Tổng hợp học phí, đã đóng, miễn giảm và còn nợ theo học kỳ.
    /// </summary>
    [HttpGet("tuition/terms/{studentCode}")]
    public async Task<IActionResult> GetTuitionTerms(string studentCode, [FromQuery] string? yearStudy, [FromQuery] string? termId, CancellationToken cancellationToken)
        => Ok(await _service.GetTuitionTermsAsync(studentCode, yearStudy, termId, cancellationToken));

    /// <summary>
    /// Chi tiết học phí theo môn; khoản thu gộp theo kỳ có isTermLevel=true.
    /// </summary>
    [HttpGet("tuition/courses/{studentCode}")]
    public async Task<IActionResult> GetTuitionCourses(string studentCode, [FromQuery] string? yearStudy, [FromQuery] string? termId, CancellationToken cancellationToken)
        => Ok(await _service.GetTuitionCoursesAsync(studentCode, yearStudy, termId, cancellationToken));

    /// <summary>
    /// Các khoản phí không phải học phí, cùng trạng thái thanh toán.
    /// </summary>
    [HttpGet("other-fees/{studentCode}")]
    public async Task<IActionResult> GetOtherFees(string studentCode, [FromQuery] string? yearStudy, [FromQuery] string? termId, CancellationToken cancellationToken)
        => Ok(await _service.GetOtherFeesAsync(studentCode, yearStudy, termId, cancellationToken));

    /// <summary>
    /// Lịch sử biên lai và các khoản đã thanh toán hợp lệ.
    /// </summary>
    [HttpGet("payments/{studentCode}")]
    public async Task<IActionResult> GetPayments(string studentCode, [FromQuery] string? yearStudy, [FromQuery] string? termId, CancellationToken cancellationToken)
        => Ok(await _service.GetPaymentsAsync(studentCode, yearStudy, termId, cancellationToken));

    /// <summary>
    /// Các khoản học phí hoặc lệ phí vẫn còn nợ.
    /// </summary>
    [HttpGet("debts/{studentCode}")]
    public async Task<IActionResult> GetDebts(string studentCode, [FromQuery] string? yearStudy, [FromQuery] string? termId, CancellationToken cancellationToken)
        => Ok(await _service.GetDebtsAsync(studentCode, yearStudy, termId, cancellationToken));

    /// <summary>
    /// Hạn đóng học phí chung và thời gian gia hạn riêng của sinh viên.
    /// </summary>
    [HttpGet("deadlines/{studentCode}")]
    public async Task<IActionResult> GetDeadlines(string studentCode, [FromQuery] string? yearStudy, [FromQuery] string? termId, CancellationToken cancellationToken)
        => Ok(await _service.GetDeadlinesAsync(studentCode, yearStudy, termId, cancellationToken));
}
