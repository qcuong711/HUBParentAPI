using Api.Application.Dtos;

namespace Api.Application;

public interface IScheduleQueryService
{
    Task<IReadOnlyList<TimetableItemDto>> GetTimetableAsync(string studentCode, string? yearStudy, string? termId);
    Task<IReadOnlyList<ExamScheduleItemDto>> GetExamsAsync(string studentCode, string? yearStudy, string? termId);
}
