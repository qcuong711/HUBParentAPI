using Api.Application.Dtos;

namespace Api.Application;

public interface IFinanceQueryService
{
    Task<IReadOnlyList<TuitionTermSummaryDto>> GetTuitionTermsAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TuitionCourseDetailDto>> GetTuitionCoursesAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OtherFeeDto>> GetOtherFeesAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentReceiptDto>> GetPaymentsAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OutstandingDebtDto>> GetDebtsAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TuitionDeadlineDto>> GetDeadlinesAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default);
}
